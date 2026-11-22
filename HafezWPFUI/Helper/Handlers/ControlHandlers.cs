using System.Windows;
using System.Windows.Input;

namespace HafezWPFUI.Helper.Handlers
{
    public static class ControlHandlers
    {
        public static bool IsKeyNumber(KeyEventArgs e)
        {
            return (e.Key >= Key.D0 && e.Key <= Key.D9) || (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9);
        }

        public static bool EnterPressed(KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
            {
                return false;
            }

            TraversalRequest tRequest = new(FocusNavigationDirection.Next);
            if (Keyboard.FocusedElement is UIElement keyboardFocus)
            {
                keyboardFocus.MoveFocus(tRequest);
            }

            return true;
        }

        public static void CheckChar(bool charMode, KeyEventArgs e)
        {
            bool isNumber = IsKeyNumber(e);
            e.Handled = charMode ? !isNumber : isNumber;
        }
    }
}
