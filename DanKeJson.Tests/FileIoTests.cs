using System;
using System.IO;
using Xunit;

namespace DanKeJson.Tests
{
    /// <summary>从文件读取 JSON / JSON5 文本。</summary>
    public class FileIoTests : IDisposable
    {
        private readonly TempWorkspace _workspace = new TempWorkspace();
        private readonly string _jsonPath;
        private readonly string _json5Path;

        public FileIoTests()
        {
            _jsonPath = _workspace.WriteFile("user.json", "{\"name\":\"DanKe\",\"age\":25,\"isVip\":true}");
            _json5Path = _workspace.WriteFile("user.json5", "{ // 注释\n name: 'DanKe', age: 25, }");
        }

        public void Dispose()
        {
            _workspace.Dispose();
        }

        [Fact]
        public void ToDataFromFile_ReturnsJsonData()
        {
            JsonData data = JSON.ToDataFromFile(_jsonPath);

            Assert.NotNull(data);
            Assert.Equal("DanKe", (string)data["name"]);
            Assert.Equal(25, (int)data["age"]);
        }

        [Fact]
        public void ToDataFromFileGeneric_ReturnsEntity()
        {
            UserModel user = JSON.ToDataFromFile<UserModel>(_jsonPath);

            Assert.Equal("DanKe", user.name);
            Assert.Equal(25, user.age);
            Assert.True(user.isVip);
        }

        [Fact]
        public void Json5ToDataFromFileGeneric_ReturnsEntity()
        {
            UserModel user = JSON5.ToDataFromFile<UserModel>(_json5Path);
            JsonData data = JSON5.ToDataFromFile(_json5Path);

            Assert.Equal("DanKe", user.name);
            Assert.Equal(25, user.age);
            Assert.Equal(25, (int)data["age"]);
        }

        [Fact]
        public void ToData_DoesNotTreatTextAsPath()
        {
            Assert.Null(JSON.ToData(_jsonPath));
            Assert.Null(JSON.ToData<UserModel>(_jsonPath));
            Assert.Null(JSON5.ToData(_jsonPath));
        }

        /// <summary>已知行为：文件不存在时直接抛出 FileNotFoundException（JSONL 则会静默返回空结果）。</summary>
        [Fact]
        public void ToDataFromFile_MissingFile_Throws()
        {
            string missingJson = _workspace.PathOf("missing.json");
            string missingJson5 = _workspace.PathOf("missing.json5");

            Assert.Throws<FileNotFoundException>(() => JSON.ToDataFromFile(missingJson));
            Assert.Throws<FileNotFoundException>(() => JSON.ToDataFromFile<UserModel>(missingJson));
            Assert.Throws<FileNotFoundException>(() => JSON5.ToDataFromFile(missingJson5));
            Assert.Throws<FileNotFoundException>(() => JSON5.ToDataFromFile<UserModel>(missingJson5));
        }
    }
}
