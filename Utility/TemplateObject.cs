namespace KRpgLib.Utility.TemplateObject
{
    public interface ITemplate { }
    public interface ITemplateObject<TTemplate>
    {
        TTemplate Template { get; }
    }
}
