// <copyright file="IndentedTextWriter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>


using System.Globalization;
using System.IO;
using System.Text;

namespace Zaaml.Text
{
	public class IndentedTextWriter : TextWriter
	{
		public const string DefaultTabString = "    ";
		private readonly TextWriter _innerWriter;
		private readonly string _tabString;
		private bool _elasticLine;
		private int _indentLevel;
		private bool _tabsPending;

		public IndentedTextWriter(TextWriter writer) : this(writer, DefaultTabString)
		{
		}

		public IndentedTextWriter(TextWriter writer, string tabString)
			: base(CultureInfo.InvariantCulture)
		{
			_innerWriter = writer;
			_tabString = tabString;
			_indentLevel = 0;
			_tabsPending = false;
		}

		public override Encoding Encoding => _innerWriter.Encoding;

		public int Indent
		{
			get => _indentLevel;
			set
			{
				if (value < 0)
					value = 0;

				_indentLevel = value;
			}
		}

		public TextWriter InnerWriter => _innerWriter;

		public override string NewLine
		{
			get => _innerWriter.NewLine;
			set => _innerWriter.NewLine = value;
		}

		internal string TabString => _tabString;

		public override void Close()
		{
			_innerWriter.Close();
		}

		public override void Flush()
		{
			_innerWriter.Flush();
		}

		internal void InternalOutputTabs()
		{
			for (var i = 0; i < _indentLevel; i++)
				_innerWriter.Write(_tabString);
		}

		public override void Write(string s)
		{
			WriteIndent();
			_innerWriter.Write(s);
		}

		public override void Write(bool value)
		{
			WriteIndent();
			_innerWriter.Write(value);
		}

		public override void Write(char value)
		{
			WriteIndent();
			_innerWriter.Write(value);
		}

		public void WriteSpace()
		{
			Write(' ');
		}

		public override void Write(char[] buffer)
		{
			WriteIndent();
			_innerWriter.Write(buffer);
		}

		public override void Write(char[] buffer, int index, int count)
		{
			WriteIndent();
			_innerWriter.Write(buffer, index, count);
		}

		public override void Write(double value)
		{
			WriteIndent();
			_innerWriter.Write(value);
		}

		public override void Write(float value)
		{
			WriteIndent();
			_innerWriter.Write(value);
		}

		public override void Write(int value)
		{
			WriteIndent();
			_innerWriter.Write(value);
		}

		public override void Write(long value)
		{
			WriteIndent();
			_innerWriter.Write(value);
		}

		public override void Write(object value)
		{
			WriteIndent();
			_innerWriter.Write(value);
		}

		public override void Write(string format, object arg0)
		{
			WriteIndent();
			_innerWriter.Write(format, arg0);
		}

		public override void Write(string format, object arg0, object arg1)
		{
			WriteIndent();
			_innerWriter.Write(format, arg0, arg1);
		}

		public override void Write(string format, params object[] arg)
		{
			WriteIndent();
			_innerWriter.Write(format, arg);
		}

		protected virtual void WriteIndent()
		{
			if (!_tabsPending)
				return;

			for (var i = 0; i < _indentLevel; i++)
				_innerWriter.Write(_tabString);

			_tabsPending = false;
		}

		public override void WriteLine(string s)
		{
			WriteIndent();
			_innerWriter.WriteLine(s);

			_tabsPending = true;
		}

		public override void WriteLine()
		{
			WriteLinePrivate();
		}

		public override void WriteLine(bool value)
		{
			WriteLinePrivate(value);
		}

		public override void WriteLine(char value)
		{
			WriteLinePrivate(value);
		}

		public override void WriteLine(char[] buffer)
		{
			WriteLinePrivate(buffer);
		}

		public override void WriteLine(char[] buffer, int index, int count)
		{
			WriteLinePrivate(buffer, index, count);
		}

		public override void WriteLine(double value)
		{
			WriteLinePrivate(value);
		}

		public override void WriteLine(float value)
		{
			WriteLinePrivate(value);
		}

		public override void WriteLine(int value)
		{
			WriteLinePrivate(value);
		}

		public override void WriteLine(long value)
		{
			WriteLinePrivate(value);
		}

		public override void WriteLine(object value)
		{
			WriteLinePrivate(value);
		}

		public override void WriteLine(string format, object arg0)
		{
			WriteLinePrivate(format, arg0);
		}

		public override void WriteLine(string format, object arg0, object arg1)
		{
			WriteLinePrivate(format, arg0, arg1);
		}

		public override void WriteLine(string format, params object[] arg)
		{
			WriteLinePrivate(format, arg);
		}

		public override void WriteLine(uint value)
		{
			WriteLinePrivate(value);
		}

		public void WriteLineWithoutIndent(string s)
		{
			_innerWriter.WriteLine(s);
		}

		public void ElasticLine()
		{
			_elasticLine = true;
		}

		private void WriteLinePrivate()
		{
			WriteIndent();
			_innerWriter.WriteLine();
			_elasticLine = false;
			_tabsPending = true;
		}

		private void WriteLinePrivate(bool value)
		{
			WriteIndent();
			_innerWriter.WriteLine(value);
			_elasticLine = false;
			_tabsPending = true;
		}

		private void WriteLinePrivate(char value)
		{
			WriteIndent();
			_innerWriter.WriteLine(value);
			_elasticLine = false;
			_tabsPending = true;
		}

		private void WriteLinePrivate(char[] buffer)
		{
			WriteIndent();
			_innerWriter.WriteLine(buffer);
			_elasticLine = false;
			_tabsPending = true;
		}

		private void WriteLinePrivate(char[] buffer, int index, int count)
		{
			WriteIndent();
			_innerWriter.WriteLine(buffer, index, count);
			_elasticLine = false;
			_tabsPending = true;
		}

		private void WriteLinePrivate(double value)
		{
			WriteIndent();
			_innerWriter.WriteLine(value);
			_elasticLine = false;
			_tabsPending = true;
		}

		private void WriteLinePrivate(float value)
		{
			WriteIndent();
			_innerWriter.WriteLine(value);
			_elasticLine = false;
			_tabsPending = true;
		}

		private void WriteLinePrivate(int value)
		{
			WriteIndent();
			_innerWriter.WriteLine(value);
			_elasticLine = false;
			_tabsPending = true;
		}

		private void WriteLinePrivate(long value)
		{
			WriteIndent();
			_innerWriter.WriteLine(value);
			_elasticLine = false;
			_tabsPending = true;
		}

		private void WriteLinePrivate(object value)
		{
			WriteIndent();
			_innerWriter.WriteLine(value);
			_elasticLine = false;
			_tabsPending = true;
		}

		private void WriteLinePrivate(string format, object arg0)
		{
			WriteIndent();
			_innerWriter.WriteLine(format, arg0);
			_elasticLine = false;
			_tabsPending = true;
		}

		private void WriteLinePrivate(string format, object arg0, object arg1)
		{
			WriteIndent();
			_innerWriter.WriteLine(format, arg0, arg1);
			_elasticLine = false;
			_tabsPending = true;
		}

		private void WriteLinePrivate(string format, params object[] arg)
		{
			WriteIndent();
			_innerWriter.WriteLine(format, arg);
			_elasticLine = false;
			_tabsPending = true;
		}


		private void WriteLinePrivate(uint value)
		{
			WriteIndent();
			_innerWriter.WriteLine(value);
			_elasticLine = false;
			_tabsPending = true;
		}

		public void WriteElasticLine()
		{
			if (_elasticLine)
				return;

			WriteLine();
		}
	}
}