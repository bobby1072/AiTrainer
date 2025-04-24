using System.Text.Json;
using AiTrainer.Web.TestBase.Models;

namespace AiTrainer.Web.TestBase.Utils;

public static class TestFaissUtils
{
    public static Task<TestFaissStoreDocuments> GetTestFaissStoreAsync()
    {
        return GetTestFaissStoreAsync(
            $".{Path.DirectorySeparatorChar}TestData{Path.DirectorySeparatorChar}docstore.json",
            $".{Path.DirectorySeparatorChar}TestData{Path.DirectorySeparatorChar}faiss.index"
        );
    }
    public static async Task<TestFaissStoreDocuments> GetTestFaissStoreAsync(string relativeDocstorePath, string relativeFaissPath)
    {
        var faissJsonDocStoreJob = File.ReadAllTextAsync(Path.GetFullPath(relativeDocstorePath));
        var faissIndexJob = File.ReadAllBytesAsync(Path.GetFullPath(relativeFaissPath));
        await Task.WhenAll(faissIndexJob, faissJsonDocStoreJob);
        
        var faissJsonDocStore = await faissJsonDocStoreJob;
        var faissIndex = await faissIndexJob;

        return new TestFaissStoreDocuments
        {
            DocStore = JsonDocument.Parse(faissJsonDocStore),
            FaissIndex = faissIndex,
        };
    } 
}