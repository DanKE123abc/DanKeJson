using Xunit;

// 部分测试会修改 JsonSettings.MaxDepth 与 CurrentCulture 这类全局状态，
// 关闭并行执行以免互相干扰（整个测试集仍在 200ms 量级内跑完）。
[assembly: CollectionBehavior(DisableTestParallelization = true)]
