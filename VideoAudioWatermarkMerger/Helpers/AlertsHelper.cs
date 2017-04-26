using System;
using UIKit;
using System.Threading.Tasks;

namespace WatermarkToVideoCreator.Helpers
{
    public static class AlertsHelper
    {
		/// <summary>
		/// Shows an informative alert.
		/// </summary>
		/// <param name="title">Title.</param>
		/// <param name="message">Message.</param>
		public static void ShowAlert(string title, string message = null)
        {
            UIApplication.SharedApplication.InvokeOnMainThread(() =>
            {
                UIAlertView alert = new UIAlertView(title, message, null, "OK", null);
                alert.AlertViewStyle = UIAlertViewStyle.Default;
                alert.Show();
            });
        }

        /// <summary>
        /// Shows an informative alert, but waits until it is dismissed.
        /// </summary>
        /// <param name="title">Title.</param>
        /// <param name="message">Message.</param>
        public static async Task ShowAlertAwait(string title, string message)
        {
            TaskCompletionSource<bool> t = new TaskCompletionSource<bool>();
            UIApplication.SharedApplication.InvokeOnMainThread(() =>
            {
                UIAlertView alert = new UIAlertView(title, message, null, "OK", null);
                alert.AlertViewStyle = UIAlertViewStyle.Default;
                alert.Show();
                alert.Clicked += (sender, e) =>
                {
                    t.SetResult(true);
                };
            });
            await t.Task;
        }

        /// <summary>
        /// Shows the alert with two options. Default is the yes and no alert.
        /// </summary>
        /// <returns>True is yes, false if no.</returns>
        /// <param name="title">Title.</param>
        /// <param name="message">Message.</param>
        /// <param name="noTxt">No button text.</param>
        /// <param name="yesTxt">Yes button text.</param>
        public static async Task<bool> ShowAlertYesNo(string title, string message = null, string noTxt = "No", string yesTxt = "Yes")
        {
            TaskCompletionSource<bool> t = new TaskCompletionSource<bool>();
            UIApplication.SharedApplication.InvokeOnMainThread(() =>
            {   
                UIAlertView alert = new UIAlertView(title, message, null, noTxt, new string[1] { yesTxt });
                alert.AlertViewStyle = UIAlertViewStyle.Default;
                alert.Show();
                alert.Clicked += (o, e) => t.SetResult(e.ButtonIndex == 1);
            });
            await t.Task;
            return t.Task.Result;
        }

		/// <summary>
		/// Returns null if cancelled.
		/// </summary>
		/// <returns>The text entered by the user. Returns null if cancelled.</returns>
		/// <param name="title">Title.</param>
		/// <param name="message">Message.</param>
		/// <param name="placeholder">Placeholder.</param>
		/// <param name="initialText">Initial text.</param>
		public static async Task<string> ShowAlertAskingText(string title, string message = null, 
                                                      string placeholder = null, string initialText = null)
        {
            UIAlertView alert = new UIAlertView(title, message, null, "Cancel", new string[1] { "OK" });
            alert.AlertViewStyle = UIAlertViewStyle.PlainTextInput;
            alert.GetTextField(0).KeyboardType = UIKeyboardType.Default;

            UITextField txtField = alert.GetTextField (0);
            txtField.Placeholder = placeholder;
            txtField.Text = initialText;

            UIApplication.SharedApplication.InvokeOnMainThread(() =>
            {
                alert.Show();
            });

            TaskCompletionSource<bool> t = new TaskCompletionSource<bool>();
            alert.Clicked += (o, e) => t.SetResult(e.ButtonIndex == 1);
            await t.Task;

			// Returns null if cancelled
			string answer = null; 
            if (t.Task.Result)
            {
                answer = alert.GetTextField(0).Text;
            }
            return answer;         
        }

    }
}

