using System.Collections.Generic;
using System.IO;
using DanKeJson.Utils;
using Xunit;

namespace DanKeJson.Tests
{
    /// <summary>工具类：注释清理、字符串转义、逐行读取、路径判断。</summary>
    public class UtilsTests : System.IDisposable
    {
        private readonly TempWorkspace _workspace = new TempWorkspace();

        public void Dispose()
        {
            _workspace.Dispose();
        }

        // ---------- CommentParser ----------

        [Fact]
        public void RemoveComments_LineCommentAtEnd()
        {
            Assert.Equal("{\"a\":1}", CommentParser.RemoveComments("{\"a\":1}// tail"));
        }

        [Fact]
        public void RemoveComments_LineCommentKeepsNewline()
        {
            string result = CommentParser.RemoveComments("// head\n{\"a\":1}\n{a:2} // tail");

            Assert.Contains("{\"a\":1}", result);
            Assert.Contains("{a:2}", result);
            Assert.DoesNotContain("head", result);
            Assert.DoesNotContain("tail", result);
        }

        [Fact]
        public void RemoveComments_BlockComment()
        {
            Assert.Equal("{\"a\":1}", CommentParser.RemoveComments("{/*c*/\"a\":1}"));
        }

        [Fact]
        public void RemoveComments_MultilineBlockComment()
        {
            string result = CommentParser.RemoveComments("{\n/* a\n b */\"a\":1}");

            Assert.Contains("\"a\":1", result);
            Assert.DoesNotContain("/*", result);
            Assert.DoesNotContain("*/", result);
        }

        [Fact]
        public void RemoveComments_KeepsCommentLikeTextInsideStrings()
        {
            const string json = "{\"u\":\"http://x//y\"}";

            Assert.Equal(json, CommentParser.RemoveComments(json));
        }

        [Fact]
        public void RemoveComments_NullOrEmpty_ReturnsInput()
        {
            Assert.Null(CommentParser.RemoveComments(null));
            Assert.Equal("", CommentParser.RemoveComments(""));
        }

        [Fact]
        public void RemoveComments_KeepsBlockCommentLikeTextInsideStrings()
        {
            Assert.Equal("{\"u\":\"a/*b*/c\"}", CommentParser.RemoveComments("{\"u\":\"a/*b*/c\"}"));
            Assert.Equal("{'u':'a/*b*/c'}", CommentParser.RemoveComments("{'u':'a/*b*/c'}"));
        }

        [Fact]
        public void RemoveComments_EscapedQuoteDoesNotEndString()
        {
            Assert.Equal("{'u':'it\\'s // x'}", CommentParser.RemoveComments("{'u':'it\\'s // x'}"));
        }

        // ---------- JsonString ----------

        [Fact]
        public void Escape_HandlesAllSpecialCharacters()
        {
            Assert.Equal("a\\\"b", JsonString.Escape("a\"b"));
            Assert.Equal("a\\\\b", JsonString.Escape("a\\b"));
            Assert.Equal("a\\nb", JsonString.Escape("a\nb"));
            Assert.Equal("a\\rb", JsonString.Escape("a\rb"));
            Assert.Equal("a\\tb", JsonString.Escape("a\tb"));
            Assert.Equal("a\\bb", JsonString.Escape("a\bb"));
            Assert.Equal("a\\fb", JsonString.Escape("a\fb"));
            Assert.Equal("a\\u0001b", JsonString.Escape("a\u0001b"));
        }

        [Fact]
        public void Escape_KeepsUnicodeAndForwardSlash()
        {
            Assert.Equal("中文/😀", JsonString.Escape("中文/😀"));
        }

        [Fact]
        public void Escape_Null_ReturnsNull()
        {
            Assert.Null(JsonString.Escape(null));
        }

        [Fact]
        public void EscapeSingleQuote_EscapesOnlySingleQuote()
        {
            Assert.Equal("it\\'s", JsonString.EscapeSingleQuote("it's"));
            Assert.Equal("a\"b", JsonString.EscapeSingleQuote("a\"b"));
            Assert.Equal("a\\nb", JsonString.EscapeSingleQuote("a\nb"));
            Assert.Null(JsonString.EscapeSingleQuote(null));
        }

        [Fact]
        public void Unquote_StripsOnlyMatchingSurroundingQuotes()
        {
            Assert.Equal("abc", JsonString.Unquote("\"abc\""));
            Assert.Equal("abc", JsonString.Unquote("abc"));
            Assert.Equal("\"", JsonString.Unquote("\""));
            Assert.Equal("", JsonString.Unquote("\"\""));
            Assert.Null(JsonString.Unquote(null));
        }

        // ---------- FileLineReader ----------

        [Fact]
        public void ReadLine_ReturnsRequestedLine()
        {
            string path = _workspace.WriteFile("lines.txt", "one\ntwo\nthree\n");

            Assert.Equal("one", FileLineReader.ReadLine(path, 1));
            Assert.Equal("two", FileLineReader.ReadLine(path, 2));
            Assert.Equal("three", FileLineReader.ReadLine(path, 3));
        }

        [Fact]
        public void ReadLine_InvalidLineNumberOrFile_ReturnsNull()
        {
            string path = _workspace.WriteFile("lines.txt", "one\n");

            Assert.Null(FileLineReader.ReadLine(path, 0));
            Assert.Null(FileLineReader.ReadLine(path, -5));
            Assert.Null(FileLineReader.ReadLine(path, 99));
            Assert.Null(FileLineReader.ReadLine(_workspace.PathOf("missing.txt"), 1));
        }

        [Fact]
        public void ReadAllLines_ReturnsAllOrEmpty()
        {
            string path = _workspace.WriteFile("lines.txt", "one\ntwo\n");

            List<string> lines = FileLineReader.ReadAllLines(path);

            Assert.Equal(new[] { "one", "two" }, lines);
            Assert.Empty(FileLineReader.ReadAllLines(_workspace.PathOf("missing.txt")));
        }

        // ---------- FilePathUtility ----------

        [Fact]
        public void IsFilePath_RequiresExistingFileWithExtension()
        {
            string existing = _workspace.WriteFile("file.json", "{}");
            string withoutExtension = _workspace.WriteFile("file", "{}");

            Assert.True(FilePathUtility.IsFilePath(existing));
            Assert.False(FilePathUtility.IsFilePath(_workspace.PathOf("missing.json")));
            Assert.False(FilePathUtility.IsFilePath(withoutExtension));
            Assert.False(FilePathUtility.IsFilePath(""));
            Assert.False(FilePathUtility.IsFilePath(null));
        }

        [Fact]
        public void Escape_EscapesUnpairedSurrogatesButKeepsPairs()
        {
            Assert.Equal("\\ud800", JsonString.Escape("\uD800"));
            Assert.Equal("\\udc00", JsonString.Escape("\uDC00"));
            Assert.Equal("😀", JsonString.Escape("😀"));
            Assert.Equal("\\ud800", JsonString.EscapeSingleQuote("\uD800"));
        }
    }
}
