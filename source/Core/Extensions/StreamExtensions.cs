//using System;
//using System.IO;

//namespace Zaaml.Core.Extensions
//{
//	public static class StreamExtensions
//	{
//		private static void ValidateStreamCopyOperation(Stream source, Stream destination)
//		{
//			if (source == null)
//				throw new ArgumentNullException("source");

//			if (destination == null)
//				throw new ArgumentNullException("destination");

//			if (!source.CanRead && !source.CanWrite)
//				throw new ObjectDisposedException("source", "ObjectDisposed_StreamClosed");

//			if (!destination.CanRead && !destination.CanWrite)
//				throw new ObjectDisposedException("destination", "ObjectDisposed_StreamClosed");

//			if (!source.CanRead)
//				throw new NotSupportedException("NotSupported_UnreadableStream");

//			if (!destination.CanWrite)
//				throw new NotSupportedException("NotSupported_UnwritableStream");

//		}
//		public static void CopyTo(this Stream source, Stream destination)
//		{
//			ValidateStreamCopyOperation(source, destination);
//			InternalCopyTo(source, destination, 81920);
//		}

//		public static void CopyTo(this Stream source, Stream destination, int bufferSize)
//		{
//			ValidateStreamCopyOperation(source, destination);
//			InternalCopyTo(source, destination, bufferSize);
//		}

//		private static void InternalCopyTo(Stream source, Stream destination, int bufferSize)
//		{
//			var buffer = new byte[bufferSize];
//			int count;

//			while ((count = source.Read(buffer, 0, buffer.Length)) != 0)
//				destination.Write(buffer, 0, count);
//		}
//	}
//}
