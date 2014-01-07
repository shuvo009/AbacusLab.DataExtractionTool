namespace AbacusLab.DataExtractionTool.Interface.Base
{
    public  interface IDownloadBase : IImplementationBase
    {
        double ProgressMaxValue { get; set; }

        double ProgressComplete { get; set; }

        bool IsIndeterminate { get; set; }
    }
}