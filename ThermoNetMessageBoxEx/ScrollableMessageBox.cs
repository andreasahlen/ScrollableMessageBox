using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class ScrollableMessageBox : Form
    {
        private MessageBoxIcon _Icon = MessageBoxIcon.Information;
        
        private MessageBoxButtons _Buttons = MessageBoxButtons.OK;

        private DialogResult _DialogResult = DialogResult.None;

        private Dictionary<ScrollableMsgBoxButtonType, string> _Localizations = new Dictionary<ScrollableMsgBoxButtonType, string>
        {
            {  ScrollableMsgBoxButtonType.OkButton, "&Ok" },
            {  ScrollableMsgBoxButtonType.CancelButton, "&Cancel" },
            {  ScrollableMsgBoxButtonType.YesButton, "&Yes" },
            {  ScrollableMsgBoxButtonType.NoButton, "&No" },
            {  ScrollableMsgBoxButtonType.AbortButton, "&Abort" },
            {  ScrollableMsgBoxButtonType.RetryButton, "&Retry" },
            {  ScrollableMsgBoxButtonType.IgnoreButton, "&Ignore" }
        };

        public enum ScrollableMsgBoxButtonType
        {
            OkButton,
            CancelButton,
            YesButton,
            NoButton,
            AbortButton,
            RetryButton,
            IgnoreButton
        }

        private Dictionary<Icon, MessageBoxIcon> _IconMapping = new Dictionary<Icon, MessageBoxIcon>
        {
            { SystemIcons.Error, MessageBoxIcon.Error },
            { SystemIcons.Warning, MessageBoxIcon.Warning },
            { SystemIcons.Information, MessageBoxIcon.Information },
            { SystemIcons.Question, MessageBoxIcon.Question },
            { SystemIcons.Exclamation, MessageBoxIcon.Error },
            { SystemIcons.Hand, MessageBoxIcon.Stop },
        };
        
        public ScrollableMessageBox(MessageBoxButtons? buttons, MessageBoxIcon? icon, string caption, string content, Dictionary<ScrollableMsgBoxButtonType, string> locales = null, int width = 512, int height = 640)
        {
            InitializeComponent();
            this.Text = caption;
            this.Width = width;
            this.Height = height;
            this.StartPosition = FormStartPosition.CenterScreen;

            if (locales == null)
            {
                this.LocalizeButtons(this._Localizations);
            }
            else
            {
                this.LocalizeButtons(locales);
            }


            if (buttons != null)
            {
                _Buttons = (MessageBoxButtons)buttons;
            }
            if (icon != null)
            {
                if (!_IconMapping.Any(v => v.Value == icon))
                {
                    throw new NotImplementedException($"Icon type '{ icon }' not implemented.");
                }
                _Icon = (MessageBoxIcon)icon;
            }

            this.FormClosing += ScrollableMessageBox_FormClosing;

            this.DrawContent(content);
        }

        private void ScrollableMessageBox_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                List<Button> definedButtons = new List<Button>();
                foreach (Control item in this.Controls)
                {
                    if (item is Button)
                    {
                        definedButtons.Add(item as Button);
                    }
                }

                if (definedButtons != null && definedButtons.Count > 0)
                {
                    foreach (Button btn in definedButtons)
                    {
                        string btName = btn.Name;
                        if (btn.Name.Contains(ScrollableMsgBoxButtonType.OkButton.ToString()))
                        {
                            this._DialogResult = DialogResult.Cancel;
                        }
                        if (btn.Name.Contains(ScrollableMsgBoxButtonType.CancelButton.ToString()))
                        {
                            this._DialogResult = DialogResult.Cancel;
                        }
                        if (btn.Name.Contains(ScrollableMsgBoxButtonType.NoButton.ToString()))
                        {
                            this._DialogResult = DialogResult.No;
                        }
                        if (btn.Name.Contains(ScrollableMsgBoxButtonType.IgnoreButton.ToString()))
                        {
                            this._DialogResult = DialogResult.Ignore;
                        }
                    }
                    
                }
            }
            
        }

        public DialogResult Response => this._DialogResult;

        private void LocalizeButtons(Dictionary<ScrollableMsgBoxButtonType, string> source)
        {
            if (source != null)
            {
                List<string> givenLocales = source.Select(v => v.Key.ToString()).ToList();                
                List<string> requiredLocales = Enum.GetNames(typeof(ScrollableMsgBoxButtonType)).ToList();

                List<string> missingLocales = requiredLocales.Except(givenLocales).ToList();

                if (missingLocales != null && missingLocales.Count > 0)
                {
                    throw new KeyNotFoundException($"locale parameter for '{missingLocales[0]}' not found");
                }

                this._Localizations = source;

            }
        }

        private void DrawContent(string content)
        {
            this.AddTextBox(content);
            this.AddButtons();
        }

        private void AddTextBox(string content)
        {
            TextBox tb = new TextBox();
            tb.Top = 10;
            tb.Left = 64;
            tb.Width = this.Width - 64 - 32;
            tb.Height = this.Height - 64 - 32;
            tb.BackColor = Color.Azure;
            tb.Multiline = true;
            tb.ScrollBars = ScrollBars.Both;
            tb.Text = content;
            this.Controls.Add(tb);
            this.Update();
        }

        private Button AddButton(ScrollableMsgBoxButtonType button, int height = 32, int width = 96)
        {
            string locale = this._Localizations.SingleOrDefault(v => v.Key == button).Value;
            Button result = new Button();
            result.Height = height;
            result.Width = width;
            result.Text = locale;
            result.Name = button.ToString();
            result.Click += Result_Click;
            return result;
        }

        private void Result_Click(object sender, EventArgs e)
        {
            if (sender is Button)
            {
                string buttonName = (sender as Button).Name;
                switch (buttonName)
                {
                    case nameof(ScrollableMsgBoxButtonType.OkButton):
                        this._DialogResult = DialogResult.OK;
                        break;
                    case nameof(ScrollableMsgBoxButtonType.CancelButton):
                        this._DialogResult = DialogResult.Cancel;
                        break;

                    case nameof(ScrollableMsgBoxButtonType.YesButton):
                        this._DialogResult = DialogResult.Yes;
                        break;
                    case nameof(ScrollableMsgBoxButtonType.NoButton):
                        this._DialogResult = DialogResult.No;
                        break;
                    case nameof(ScrollableMsgBoxButtonType.AbortButton):
                        this._DialogResult = DialogResult.Abort;
                        break;

                    case nameof(ScrollableMsgBoxButtonType.RetryButton):
                        this._DialogResult = DialogResult.Retry;
                        break;

                    case nameof(ScrollableMsgBoxButtonType.IgnoreButton):
                        this._DialogResult = DialogResult.Ignore;
                        break;

                    default:
                        break;
                }
                this.Visible = false;
            }
        }

        private void AddButtons()
        {
            List<Button> buttons = new List<Button>();
            switch (this._Buttons)
            {
                case MessageBoxButtons.OK:
                    buttons.Add(this.AddButton(ScrollableMsgBoxButtonType.OkButton));
                    break;
                case MessageBoxButtons.OKCancel:
                    buttons.Add(this.AddButton(ScrollableMsgBoxButtonType.OkButton));
                    buttons.Add(this.AddButton(ScrollableMsgBoxButtonType.CancelButton));
                    break;
                case MessageBoxButtons.AbortRetryIgnore:
                    buttons.Add(this.AddButton(ScrollableMsgBoxButtonType.AbortButton));
                    buttons.Add(this.AddButton(ScrollableMsgBoxButtonType.RetryButton));
                    buttons.Add(this.AddButton(ScrollableMsgBoxButtonType.IgnoreButton));
                    break;
                case MessageBoxButtons.YesNoCancel:
                    buttons.Add(this.AddButton(ScrollableMsgBoxButtonType.YesButton));
                    buttons.Add(this.AddButton(ScrollableMsgBoxButtonType.NoButton));
                    buttons.Add(this.AddButton(ScrollableMsgBoxButtonType.CancelButton));
                    break;
                case MessageBoxButtons.YesNo:
                    buttons.Add(this.AddButton(ScrollableMsgBoxButtonType.YesButton));
                    buttons.Add(this.AddButton(ScrollableMsgBoxButtonType.NoButton));
                    break;
                case MessageBoxButtons.RetryCancel:
                    buttons.Add(this.AddButton(ScrollableMsgBoxButtonType.RetryButton));
                    buttons.Add(this.AddButton(ScrollableMsgBoxButtonType.CancelButton));
                    break;
                default:
                    break;
            }
            foreach (Button btn in buttons)
            {
                int index = buttons.IndexOf(btn) + 1;

                btn.Left = (64 * index) + (buttons.IndexOf(btn) * btn.Width);

                btn.Top = this.Height - 48 - btn.Height;
                this.Controls.Add(btn);
                this.Update();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            KeyValuePair<Icon, MessageBoxIcon> selection = this._IconMapping.First(v => v.Value == this._Icon);
            // e.Graphics.DrawIcon(SystemIcons.Error, 10, 10);
            e.Graphics.DrawIcon(selection.Key, 10, 10);
            base.OnPaint(e);

        }

        public void Dispose()
        {
            base.Dispose();
        }
    }
}
