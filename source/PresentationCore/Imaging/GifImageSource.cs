using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Zaaml.Core;

#pragma warning disable 675
#pragma warning disable 414
#pragma warning disable 169

namespace Zaaml.PresentationCore.Imaging
{
	public class GifImageSource
	{
		private readonly MemoryStream _gifStream;
		private bool _isValid;
		private readonly GifPalette _globalPalette;
		private readonly LogicalScreenDescriptor _sreenDescriptor;
		private readonly List<GifFrame> _frames = new List<GifFrame>();

		public GifImageSource(Stream stream)
		{
			try
			{
				_gifStream = new MemoryStream();
				stream.CopyTo(_gifStream);
				_gifStream.Seek(0, SeekOrigin.Begin);

				using (var reader = new BinaryReader(_gifStream))
				{
					var gifSign = new string(reader.ReadChars(6));

					var gif89a = string.Equals(gifSign, "gif89a", StringComparison.OrdinalIgnoreCase);
					var gif87a = string.Equals(gifSign, "gif87a", StringComparison.OrdinalIgnoreCase);

					if ((gif87a || gif89a) == false)
						return;

					_sreenDescriptor = LogicalScreenDescriptor.Read(reader);
					_globalPalette = _sreenDescriptor.HasGlobalPalette ? GifPalette.Read(reader, _sreenDescriptor.GlobalPaletteColorCount) : null;

					while (true)
					{
						var ch = reader.ReadChar();

						if (ch == ';' && reader.BaseStream.Position == reader.BaseStream.Length)
							break;

						switch (ch)
						{
							case '!':
								SkipExtension(reader);
								break;
							case ',':
								_frames.Add(ReadImage(reader));
								break;
						}
					}
				}

				_isValid = true;
			}
			catch (Exception)
			{
				_isValid = false;
			}
		}

		public ImageSource GetFrame(int index)
		{
			return _frames[index].WriteableBmp;
		}

		public int FrameCount => _frames.Count;

	  private GifFrame ReadImage(BinaryReader reader)
		{

			return ReadImageData(reader);
		}

		private GifFrame ReadImageData(BinaryReader reader)
		{
			var imageDescriptor = ImageDescriptor.Read(reader);
			var localPalette = imageDescriptor.HasLocalPalette ? GifPalette.Read(reader, imageDescriptor.LocalPaletteColorCount) : null;

			var lzwInitialCodeSize = reader.ReadByte() + 1;

			var lzwBitReader = new GifLzwBitReader(new GifLzwByteReader(reader));

			var lzwCodeSize = lzwInitialCodeSize;
			var nextIncValue = (2 << (lzwCodeSize - 1)) - 1;
			var decodeTable = new GifDecodeTable(_sreenDescriptor.GlobalPaletteColorCount);

			int value;

#if SILVERLIGHT
		  var writeableBmp = new WriteableBitmap(_sreenDescriptor.Width, _sreenDescriptor.Height);
#else
		  var writeableBmp = new WriteableBitmap(_sreenDescriptor.Width, _sreenDescriptor.Height, 96.0, 96.0, PixelFormats.Bgra32, null);
#endif

      var bmpWriter = new BmpWriter(writeableBmp, _globalPalette);

			while (lzwBitReader.Read(lzwCodeSize, out value))
			{
				if (decodeTable.Count == nextIncValue)
				{
					lzwCodeSize++;
					nextIncValue = (2 << (lzwCodeSize - 1)) - 1;
				}

				bool reset;
				if (decodeTable.ProcessNextCode(value, bmpWriter, out reset) == false)
					break;

				if (reset)
				{
					lzwCodeSize = lzwInitialCodeSize;
					nextIncValue = (2 << (lzwCodeSize - 1)) - 1;
				}

				if (lzwCodeSize > 12)
					lzwCodeSize = 12;
			}

		  bmpWriter.Flush();

      return new GifFrame(imageDescriptor, localPalette, writeableBmp);

		}

		private void SkipExtension(BinaryReader reader)
		{
			var extensionKind = reader.ReadByte();
			while (true)
			{
				var dataSize = reader.ReadByte();
				if (dataSize == 0)
					break;

				reader.ReadBytes(dataSize);
			}
		}

		private class BmpWriter : IByteWriter
		{
			private readonly WriteableBitmap _bitmap;
			private readonly GifPalette _palette;
			private int _index;

#if !SILVERLIGHT

		  private readonly byte[] _buffer;

#endif

      public BmpWriter(WriteableBitmap bitmap, GifPalette palette)
			{
				_bitmap = bitmap;
				_palette = palette;

#if !SILVERLIGHT

			  _buffer = new byte[(int)_bitmap.Height * (int)_bitmap.Width *  4];
#endif
      }


			public void WriteByte(byte value)
			{
#if SILVERLIGHT
				_bitmap.Pixels[_index++] = _palette.GetColor(value);
#else

			  var intColor = _palette.GetColor(value);

			  _buffer[_index++] = (byte)(0xFF & intColor);
			  intColor >>= 8;

			  _buffer[_index++] = (byte)(0xFF & intColor);
			  intColor >>= 8;

			  _buffer[_index++] = (byte)(0xFF & intColor);
			  intColor >>= 8;

			  _buffer[_index++] = (byte)(0xFF & intColor);
#endif
			}

		  public void Flush()
		  {
#if !SILVERLIGHT
		    var rect = new Int32Rect(0, 0, (int)_bitmap.Width, (int)_bitmap.Height);
		    _bitmap.WritePixels(rect, _buffer, 4 * (int)_bitmap.Width, 0);
#endif
      }
    }

		private class GifLzwByteReader
		{
			private readonly BinaryReader _reader;
			private int _index = -1;
			private byte[] _buffer;

			public GifLzwByteReader(BinaryReader reader)
			{
				_reader = reader;
			}

			public bool Read(out byte value)
			{
				if (_index == -1 || _index == _buffer.Length)
				{
					var count = _reader.ReadByte();
					if (count == 0)
					{
						value = 0;
						return false;
					}

					_buffer = _reader.ReadBytes(count);
					_index = 0;
				}

				value = _buffer[_index++];
				return true;
			}
		}

		private class GifLzwBitReader
		{
			private readonly GifLzwByteReader _byteReader;
			private byte _currentByte;
			private int _curBitIndex = -1;
			private bool _isEOF;

			public GifLzwBitReader(GifLzwByteReader byteReader)
			{
				_byteReader = byteReader;
			}

			public bool Read(int lzwCodeSize, out int value)
			{
				long res = 0;
				var resBitIndex = 0;

				var bits = lzwCodeSize;

				while (true)
				{
					if (_curBitIndex == -1 && !_isEOF)
					{
						if (_byteReader.Read(out _currentByte) == false)
						{
							_currentByte = 0;
							_isEOF = true;
						}

						_curBitIndex = 0;
					}

					if (_curBitIndex == 0)
					{
						if (bits < 8)
						{
							var mask = ~(~0 << bits);
							res |= (_currentByte & mask) << resBitIndex;

							_curBitIndex += bits;
							break;
						}

						res |= _currentByte << resBitIndex;
						resBitIndex += 8;
						bits -= 8;
						_curBitIndex = -1;
					}
					else
					{
						var bitsLeft = 8 - _curBitIndex;

						var mask = ~(~0 << bits);
						res |= ((_currentByte >> _curBitIndex) & mask) << resBitIndex;

						if (bits < bitsLeft)
						{
							_curBitIndex += bits;
							break;
						}

						_curBitIndex = -1;
						bits -= bitsLeft;
						resBitIndex += bitsLeft;
					}
				}

				value = (int)res;
				return _isEOF == false || (_isEOF && _curBitIndex >= 0);
			}
		}

		internal struct LogicalScreenDescriptor
		{
			public short Width;
			public short Height;
			public byte Flags;
			public byte BackgroundColorIndex;
			public byte AspectRatio;

			public bool HasGlobalPalette => (Flags & 0x80) != 0;

		  public int OriginalImageColorDepth => ((Flags >> 4) & 0x7 + 1) * 3;

		  public bool IsPaletteSorted => (Flags & 0x8) != 0;

		  public int GlobalPaletteColorCount => (int)Math.Pow(2, (Flags & 0x7) + 1);

		  public static LogicalScreenDescriptor Read(BinaryReader reader)
			{
				return new LogicalScreenDescriptor
				{
					Width = reader.ReadInt16(),
					Height = reader.ReadInt16(),
					Flags = reader.ReadByte(),
					BackgroundColorIndex = reader.ReadByte(),
					AspectRatio = reader.ReadByte()
				};
			}
		}

		internal struct ImageDescriptor
		{
			public short Left;
			public short Top;
			public short Width;
			public short Height;
			public byte Flags;

			public bool HasLocalPalette => (Flags & 0x80) != 0;

		  public bool Interlaced => (Flags & 0x40) != 0;

		  public bool IsPaletteSorted => (Flags & 0x20) != 0;

		  public int LocalPaletteColorCount => (int)Math.Pow(2, (Flags & 0x7) + 1);

		  public static ImageDescriptor Read(BinaryReader reader)
			{
				return new ImageDescriptor
				{
					Left = reader.ReadInt16(),
					Top = reader.ReadInt16(),
					Width = reader.ReadInt16(),
					Height = reader.ReadInt16(),
					Flags = reader.ReadByte()
				};
			}
		}

		internal class GifPalette
		{
			private readonly List<int> _colors = new List<int>();

			private GifPalette()
			{
			}

			public int GetColor(int index)
			{
				return _colors[index];
			}

			public static GifPalette Read(BinaryReader reader, int colorsCount)
			{
				var palette = new GifPalette();

				for (var i = 0; i < colorsCount; i++)
				{
					var r = reader.ReadByte();
					var g = reader.ReadByte();
					var b = reader.ReadByte();

					palette._colors.Add((0xFF << 24) | (r << 16) | (g << 8) | b);
				}

				return palette;
			}
		}

		internal interface IByteReader
		{
			bool IsEnd { get; }
			byte ReadByte();
		}

		internal interface IByteWriter
		{
			void WriteByte(byte value);
		  void Flush();
		}

		internal class GifDecodeTable
		{
			private readonly int _size;
			private readonly List<GifEntry> _table = new List<GifEntry>();
			private readonly List<byte> _buffer = new List<byte>();
			private int _oldCode;
			private int _resetTable;
			private int _endOfStream;

			public GifDecodeTable(int size)
			{
				_size = size;
				_oldCode = -1;
				Reset();
			}

			public void Reset()
			{
				_resetTable = _size;
				_endOfStream = _resetTable + 1;
				_oldCode = -1;

				_table.Clear();
				_table.AddRange(Enumerable.Range(0, _size).Select(ch => new GifEntry(-1, (byte)ch)));
				_table.Add(new GifEntry(-1, 0xff));
				_table.Add(new GifEntry(-1, 0xff));
			}

			public int Count => _table.Count;

		  private void Add(int prefix, byte value)
			{
				_table.Add(new GifEntry(prefix, value));
			}

			private byte WriteData(int index, IByteWriter writer)
			{
				_buffer.Clear();

				while (true)
				{
					var entry = _table[index];
					_buffer.Add(entry.Value);
					index = entry.PrefixIndex;
					if (index != -1) continue;

					foreach (var dataByte in _buffer.AsEnumerable().Reverse())
						writer.WriteByte(dataByte);

					return _buffer.Last();
				}
			}

			public bool ProcessNextCode(int code, IByteWriter writer, out bool reset)
			{
				reset = false;

				if (code == _resetTable)
				{
					reset = true;
					Reset();
				}
				else if (code == _endOfStream)
				{
					return false;
				}
				else if (_oldCode == -1)
				{
					WriteData(code, writer);
					_oldCode = code;
				}
				else if (code < Count)
				{
					var fbyte = WriteData(code, writer);

					Add(_oldCode, fbyte);
					_oldCode = code;
				}
				else
				{
					var fbyte = WriteData(_oldCode, writer);
					writer.WriteByte(fbyte);

					Add(_oldCode, fbyte);
					_oldCode = code;
				}

				return true;
			}
		}

		internal struct GifEntry
		{
			public GifEntry(int prefixIndex, byte value)
			{
				PrefixIndex = prefixIndex;
				Value = value;
			}

			public readonly int PrefixIndex;
			public readonly byte Value;
		}

		private class GifFrame
		{
			private readonly ImageDescriptor _imageDescriptor;
			private readonly GifPalette _localPalette;
			private readonly WriteableBitmap _writeableBmp;

			public GifFrame(ImageDescriptor imageDescriptor, GifPalette localPalette, WriteableBitmap writeableBmp)
			{
				_imageDescriptor = imageDescriptor;
				_localPalette = localPalette;
				_writeableBmp = writeableBmp;
			}

			public WriteableBitmap WriteableBmp => _writeableBmp;
		}
	}

	public class GifImageSourceConverter : IValueConverter
	{
		public int Frame { get; set; }

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			try
			{
				var bytes = value as byte[];
				var stream = value as Stream;
				return GetImageSource(bytes != null ? new MemoryStream(bytes) : stream, Frame);
			}
			catch (Exception e)
			{
        LogService.LogError(e);
			}

			return null;
		}

		private static ImageSource GetImageSource(Stream stream, int frame)
		{
			if (stream == null)
				return null;

			var gifImage = new GifImageSource(stream);
			return frame < gifImage.FrameCount ? gifImage.GetFrame(frame) : null;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
