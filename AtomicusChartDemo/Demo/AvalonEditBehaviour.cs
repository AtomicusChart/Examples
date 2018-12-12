using System;
using System.Windows;
using System.Windows.Interactivity;
using ICSharpCode.AvalonEdit;

namespace AtomicusChart.Demo
{
	public sealed class AvalonEditBehaviour : Behavior<TextEditor>
	{
		public static readonly DependencyProperty GiveMeTheTextProperty =
			DependencyProperty.Register(
				nameof(MyText),
				typeof(string),
				typeof(AvalonEditBehaviour),
				new FrameworkPropertyMetadata(
					default(string),
					FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
					PropertyChangedCallback));
	
		public string MyText
		{
			get => (string)GetValue(GiveMeTheTextProperty);
			set => SetValue(GiveMeTheTextProperty, value);
		}
	
		protected override void OnAttached()
		{
			base.OnAttached();
			if (AssociatedObject != null)
				AssociatedObject.TextChanged += AssociatedObjectOnTextChanged;
		}
	
		protected override void OnDetaching()
		{
			base.OnDetaching();
			if (AssociatedObject != null)
				AssociatedObject.TextChanged -= AssociatedObjectOnTextChanged;
		}
	
		private void AssociatedObjectOnTextChanged(object sender, EventArgs eventArgs)
		{
			var textEditor = sender as TextEditor;
			if (textEditor?.Document != null)
				MyText = textEditor.Document.Text;
		}
	
		private static void PropertyChangedCallback(
			DependencyObject dependencyObject,
			DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
		{
			string newString = dependencyPropertyChangedEventArgs.NewValue?.ToString() ?? string.Empty;
			var behavior = dependencyObject as AvalonEditBehaviour;
			TextEditor editor = behavior?.AssociatedObject;
			if (editor?.Document == null || editor.Document.Text == newString)
				return;
			else
			{
				int caretOffset = editor.CaretOffset;
				editor.Document.Text = newString;
				editor.CaretOffset = Math.Min(newString.Length, caretOffset);
			}
		}
	}
}
