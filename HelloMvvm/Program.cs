using System;
using System.Collections.Generic;
using System.Linq;
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

            var input = new TextInput();
            input.PlaceHolder = "Enter post";
            SimpleBinding.Create(button, "CommandParameter", input, "Value");

            var list = new ItemsControl<string>(new VBox());
            SimpleBinding.Create(list, "ItemsSource", mainViewModel, "Posts");

            var box = new VBox
            {
                Content = { input, button, list }
            };

            Element.GetById("target").AppendChild(box);
        }
    }
}
