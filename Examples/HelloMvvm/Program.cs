using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using JSIL.Dom;
using JSIL.Ui;
using JSIL.Ui.Mvvm;
using JSIL.Ui.Input;

namespace HelloMvvm
{
    public class Program
    {
        public static void Main()
        {
            var mainViewModel = new MainViewModel();

            var button = new Button()
            {
                Command = mainViewModel.PostCommand,
                TextContent = "Post"
            };

            var headerInput = new TextInput();
            headerInput.PlaceHolder = "Header";
            SimpleBinding.Create(mainViewModel, "HeaderInput", headerInput, "Value");

            var contentInput = new TextInput();
            contentInput.PlaceHolder = "Content";
            SimpleBinding.Create(mainViewModel, "ContentInput", contentInput, "Value");

            var list = new ItemsControl<Post>(new VBox())
            {
                // "postItemTemplate" is a reference to an element defined in MainPage.html
                ItemElementFactory = new TemplateElementFactory<Post>("postItemTemplate")
            };
            SimpleBinding.Create(list, "ItemsSource", mainViewModel, "Posts");

            var box = new VBox
            {
                Content = { headerInput, contentInput, button, list }
            };

            Element.GetById("target").AppendChild(box);
        }
    }
}
