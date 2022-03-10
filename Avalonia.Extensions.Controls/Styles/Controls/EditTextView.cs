using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Logging;
using Avalonia.Markup.Xaml;
using System;

namespace Avalonia.Extensions.Controls
{
    public partial class EditTextView : TemplatedControl
    {
        protected override void OnInitialized()
        {
            base.OnInitialized();
            ApplyStyle();
        }
        private void ApplyStyle()
        {
            try
            {
                var xaml = @"<Border  xmlns='https://github.com/avaloniaui' xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' Name='border'
                Background ='{TemplateBinding Background}'
                BorderBrush='{TemplateBinding BorderBrush}'
                BorderThickness='{TemplateBinding BorderThickness}'
                CornerRadius='{TemplateBinding CornerRadius}'>
          <DockPanel Margin='{TemplateBinding Padding}'
                     HorizontalAlignment='{TemplateBinding HorizontalContentAlignment}'
                     VerticalAlignment='{TemplateBinding VerticalContentAlignment}'>
            <TextBlock Name='floatingWatermark'
                       Foreground='{DynamicResource ThemeAccentBrush}'
                       FontSize='{DynamicResource FontSizeSmall}'
                       Text='{TemplateBinding Watermark}'
                       DockPanel.Dock='Top'>
              <TextBlock.IsVisible>
                <MultiBinding Converter='{x:Static BoolConverters.And}'> 
                   <Binding RelativeSource='{RelativeSource TemplatedParent}'
                           Path='UseFloatingWatermark' />
                  <Binding RelativeSource='{RelativeSource TemplatedParent}'
                           Path='Text'
                           Converter='{x:Static StringConverters.IsNotNullOrEmpty}' />
                </MultiBinding>
              </TextBlock.IsVisible>
            </TextBlock>
            <DataValidationErrors>
              <Grid ColumnDefinitions='Auto,*,Auto'>
                  <ContentPresenter Grid.Column='0' Grid.ColumnSpan='1' Content='{TemplateBinding InnerLeftContent}' />      
                      <ScrollViewer Grid.Column='1' Grid.ColumnSpan='1'
                              HorizontalScrollBarVisibility='{TemplateBinding (ScrollViewer.HorizontalScrollBarVisibility)}'
                              VerticalScrollBarVisibility='{TemplateBinding (ScrollViewer.VerticalScrollBarVisibility)}'
                              IsScrollChainingEnabled='{TemplateBinding (ScrollViewer.IsScrollChainingEnabled)}'
                              AllowAutoHide='{TemplateBinding (ScrollViewer.AllowAutoHide)}'>
                  <Panel>
                    <TextBlock Name='watermark'
                              Opacity='0.5'
                              Text='{TemplateBinding Watermark}'
                              TextAlignment='{TemplateBinding TextAlignment}'
                              TextWrapping='{TemplateBinding TextWrapping}'
                              IsVisible='{TemplateBinding Text, Converter={x:Static StringConverters.IsNullOrEmpty}}' />
                    <TextPresenter Name='PART_TextPresenter'
                                    Text='{TemplateBinding Text, Mode=TwoWay}'
                                    CaretIndex='{TemplateBinding CaretIndex}'
                                    SelectionStart='{TemplateBinding SelectionStart}'
                                    SelectionEnd='{TemplateBinding SelectionEnd}'
                                    TextAlignment='{TemplateBinding TextAlignment}'
                                    TextWrapping='{TemplateBinding TextWrapping}'
                                    PasswordChar='{TemplateBinding PasswordChar}'
                                    RevealPassword='{TemplateBinding RevealPassword}'
                                    SelectionBrush='{TemplateBinding SelectionBrush}'
                                    SelectionForegroundBrush='{TemplateBinding SelectionForegroundBrush}'
                                    CaretBrush='{TemplateBinding CaretBrush}' />
                  </Panel>
                </ScrollViewer>
                <ContentPresenter Grid.Column='2' Grid.ColumnSpan='1' Content='{TemplateBinding InnerRightContent}' />     
                   </Grid>
                 </DataValidationErrors>
               </DockPanel>
             </Border>";
                var border = AvaloniaRuntimeXamlLoader.Parse<Border>(xaml);
                Template = new FuncControlTemplate((_, _) => border);
                ApplyTemplate();
            }
            catch (Exception ex)
            {
                Logger.TryGet(LogEventLevel.Error, LogArea.Control)?.Log(this, ex.Message);
            }
        }
    }
}