using System.Collections.Generic;
using System.IO;
using Xunit;

namespace DanKeJson.Tests
{
    /// <summary>JSONL：.jsonl 文件按行读写。</summary>
    public class JsonlTests : IClassFixture<JsonlTests.Fixture>
    {
        private readonly Fixture _fixture;

        public JsonlTests(Fixture fixture)
        {
            _fixture = fixture;
        }

        public sealed class Fixture : System.IDisposable
        {
            public TempWorkspace Workspace { get; } = new TempWorkspace();

            /// <summary>两行内容：一个对象、一个数组。</summary>
            public string TwoLines { get; }

            /// <summary>含空行的文件。</summary>
            public string WithBlankLines { get; }

            public Fixture()
            {
                TwoLines = Workspace.WriteFile("two.jsonl", "{\"a\":1}\n[1,2]\n");
                WithBlankLines = Workspace.WriteFile("blank.jsonl", "{\"age\":1}\n\n   \n{\"age\":2}\n");
            }

            public void Dispose()
            {
                Workspace.Dispose();
            }
        }

        [Fact]
        public void AllLineToData_ReadsEveryLine()
        {
            List<JsonData> lines = JSONL.AllLineToData(_fixture.TwoLines);

            Assert.Equal(2, lines.Count);
            Assert.Equal(JsonData.Type.Object, lines[0].type);
            Assert.Equal(JsonData.Type.Array, lines[1].type);
            Assert.Equal(1, (int)lines[0]["a"]);
            Assert.Equal(2, lines[1].array.Count);
        }

        [Fact]
        public void AllLineToData_SkipsBlankLines()
        {
            List<UserModel> users = JSONL.AllLineToData<UserModel>(_fixture.WithBlankLines);

            Assert.Equal(2, users.Count);
            Assert.Equal(1, users[0].age);
            Assert.Equal(2, users[1].age);
        }

        [Fact]
        public void AllLineToData_MissingOrInvalidFile_ReturnsEmptyList()
        {
            Assert.Empty(JSONL.AllLineToData(_fixture.Workspace.PathOf("missing.jsonl")));
            Assert.Empty(JSONL.AllLineToData<UserModel>(_fixture.Workspace.PathOf("missing.jsonl")));
            Assert.Empty(JSONL.AllLineToData(_fixture.Workspace.PathOf("no-extension")));
        }

        [Fact]
        public void LineToData_ReadsSpecificLine()
        {
            JsonData first = JSONL.LineToData(_fixture.TwoLines, 1);
            JsonData second = JSONL.LineToData(_fixture.TwoLines, 2);

            Assert.Equal(JsonData.Type.Object, first.type);
            Assert.Equal(JsonData.Type.Array, second.type);
            Assert.Equal(1, (int)first["a"]);
        }

        [Fact]
        public void LineToData_OutOfRangeOrInvalidNumber_ReturnsNull()
        {
            Assert.Null(JSONL.LineToData(_fixture.TwoLines, 99));
            Assert.Null(JSONL.LineToData(_fixture.TwoLines, 0));
            Assert.Null(JSONL.LineToData(_fixture.TwoLines, -1));
            Assert.Null(JSONL.LineToData(_fixture.Workspace.PathOf("missing.jsonl"), 1));
        }

        [Fact]
        public void LineToDataGeneric_ReadsEntity()
        {
            UserModel user = JSONL.LineToData<UserModel>(_fixture.Workspace.WriteFile("user.jsonl", "{\"name\":\"DanKe\",\"age\":9}\n"), 1);

            Assert.Equal("DanKe", user.name);
            Assert.Equal(9, user.age);
            Assert.Null(JSONL.LineToData<UserModel>(_fixture.TwoLines, 99));
        }

        [Fact]
        public void ListToJson_ReturnsJoinedLines()
        {
            var list = new List<JsonData> { JSON.ToData("{\"a\":1}"), JSON.ToData("[1,2]") };

            Assert.Equal("{\"a\":1}\n[1,2]", JSONL.ListToJson(list));
        }

        [Fact]
        public void ListToJson_WritesFileWithOneLinePerRecord()
        {
            var list = new List<JsonData> { JSON.ToData("{\"a\":1}"), JSON.ToData("[1,2]") };
            string path = _fixture.Workspace.PathOf("out.jsonl");

            string returned = JSONL.ListToJson(list, path);

            Assert.Equal("{\"a\":1}\n[1,2]", returned);
            Assert.Equal("{\"a\":1}\n[1,2]\n", File.ReadAllText(path).Replace("\r\n", "\n"));
        }

        [Fact]
        public void ListToJsonGeneric_WritesEntities()
        {
            var users = new List<UserModel>
            {
                new UserModel { name = "A", age = 1, isVip = false },
                new UserModel { name = "B", age = 2, isVip = true }
            };
            string path = _fixture.Workspace.PathOf("users.jsonl");

            JSONL.ListToJson(users, path);

            Assert.Equal(
                "{\"name\":\"A\",\"age\":1,\"isVip\":false}\n{\"name\":\"B\",\"age\":2,\"isVip\":true}\n",
                File.ReadAllText(path).Replace("\r\n", "\n"));

            List<UserModel> restored = JSONL.AllLineToData<UserModel>(path);
            Assert.Equal(2, restored.Count);
            Assert.True(restored[1].isVip);
        }

        [Fact]
        public void ListToJson_NullList_ReturnsNull()
        {
            Assert.Null(JSONL.ListToJson((List<JsonData>)null));
            Assert.Null(JSONL.ListToJson((List<UserModel>)null));
        }

        /// <summary>写文件时会转义行内换行，保证一条记录占一行。</summary>
        [Fact]
        public void ListToJson_EscapesNewlinesInsideARecord()
        {
            var tricky = new JsonData(JsonData.Type.Number) { json = "1\n2" };
            string path = _fixture.Workspace.PathOf("escaped.jsonl");

            JSONL.ListToJson(new List<JsonData> { tricky }, path);

            Assert.Equal("1\\n2\n", File.ReadAllText(path).Replace("\r\n", "\n"));
        }
    }
}
