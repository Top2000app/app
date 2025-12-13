namespace Top2000MauiApp.Globalisation;

public class TranslateExtension : IMarkupExtension<BindingBase>
{
    public TranslateExtension(string text)
    {
        this.Text = text;
    }

    public string Text { get; set; }

    object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider)
    {
        return this.ProvideValue(serviceProvider);
    }

    public BindingBase ProvideValue(IServiceProvider serviceProvider)
    {
        var binding = new Binding
        {
            Mode = BindingMode.OneWay,
            Path = $"[{this.Text}]",
            Source = Translator.Instance,
        };
        return binding;
    }
}
