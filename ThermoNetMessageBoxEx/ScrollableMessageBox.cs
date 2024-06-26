using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class ScrollableMessageBox : Form
    {
        private MessageBoxIcon _Icon = MessageBoxIcon.Information;

        private MessageBoxButtons _Buttons = MessageBoxButtons.OK;

        private DialogResult _DialogResult = DialogResult.None;

        private List<Button> _ButtonControls = new List<Button>();

        private TextBox _TextBox = new TextBox();

        private bool _AllowDirectKeyInput = false;

        private Dictionary<ScrollableMsgBoxButtonType, Keys> _KeyMapping = new Dictionary<ScrollableMsgBoxButtonType, Keys>();

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

        public ScrollableMessageBox(MessageBoxButtons? buttons, MessageBoxIcon? icon, string caption, string content, bool allowDirectKeyInput, Dictionary<ScrollableMsgBoxButtonType, string> locales = null, int width = 512, int height = 640)
        {
            InitializeComponent();
            this.Text = caption;
            this.Width = width;
            this.Height = height;
            this.BackColor = Color.FromKnownColor(KnownColor.Window);
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

            this.EventHandlers(true);

            if (allowDirectKeyInput)
            {
                this._AllowDirectKeyInput = allowDirectKeyInput;
                this.AssignHotkeys();
            }

            this.DrawContent(content);
        }

        private void EventHandlers(bool attach)
        {
            if (attach)
            {
                this.FormClosing += ScrollableMessageBox_FormClosing;
                this._TextBox.KeyUp += ScrollableMessageBox_KeyUp;
            }
            else
            {
                this.FormClosing -= ScrollableMessageBox_FormClosing;
                this._ButtonControls.ForEach(v => v.Click -= this.Result_Click);
                this._ButtonControls.ForEach(v => v.KeyUp -= this.ScrollableMessageBox_KeyUp);
                this._TextBox.KeyUp -= ScrollableMessageBox_KeyUp;
            }
        }

        private void ScrollableMessageBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (this._AllowDirectKeyInput)
            {
                if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Return)
                {
                    if (this._ButtonControls.Any(v => v.Name.ToString().Contains(ScrollableMsgBoxButtonType.OkButton.ToString())))
                    {
                        this._DialogResult = DialogResult.OK; // Default on RETURN / ENTER pressed
                        this.HideDialog();
                    }
                }
                else if (e.KeyCode == Keys.Escape)
                {
                    if (this._ButtonControls.Count == 1 && this._ButtonControls.Any(v => v.Name.ToString().Contains(ScrollableMsgBoxButtonType.OkButton.ToString())))
                    {
                        this._DialogResult = DialogResult.Cancel; // Default on ESC pressed
                        this.HideDialog();
                    }
                    else if (this._ButtonControls.Any(v => v.Name.ToString().Contains(ScrollableMsgBoxButtonType.CancelButton.ToString())))
                    {
                        this._DialogResult = DialogResult.Cancel; // Default on ESC pressed
                        this.HideDialog();
                    }
                }
                else
                {
                    if (this._KeyMapping.Any(v => v.Value == e.KeyCode))
                    {
                        KeyValuePair<ScrollableMsgBoxButtonType, Keys> key = this._KeyMapping.FirstOrDefault(v => v.Value == e.KeyCode);
                        if (key.Key.ToString().Contains(ScrollableMsgBoxButtonType.OkButton.ToString()))
                        {
                            this._DialogResult = DialogResult.OK;
                        }
                        else if (key.Key.ToString().Contains(ScrollableMsgBoxButtonType.CancelButton.ToString()))
                        {
                            this._DialogResult = DialogResult.Cancel;
                        }
                        else if (key.Key.ToString().Contains(ScrollableMsgBoxButtonType.YesButton.ToString()))
                        {
                            this._DialogResult = DialogResult.Yes;
                        }
                        else if (key.Key.ToString().Contains(ScrollableMsgBoxButtonType.NoButton.ToString()))
                        {
                            this._DialogResult = DialogResult.No;
                        }
                        else if (key.Key.ToString().Contains(ScrollableMsgBoxButtonType.AbortButton.ToString()))
                        {
                            this._DialogResult = DialogResult.Abort;
                        }
                        else if (key.Key.ToString().Contains(ScrollableMsgBoxButtonType.RetryButton.ToString()))
                        {
                            this._DialogResult = DialogResult.Retry;
                        }
                        else if (key.Key.ToString().Contains(ScrollableMsgBoxButtonType.IgnoreButton.ToString()))
                        {
                            this._DialogResult = DialogResult.Ignore;
                        }
                        this.HideDialog();
                    }
                }
            }
        }

        private void ScrollableMessageBox_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                this._DialogResult = DialogResult.Cancel; // Default on Close
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

        private void AssignHotkeys()
        {
            foreach (KeyValuePair<ScrollableMsgBoxButtonType, string> item in this._Localizations)
            {
                this._KeyMapping.Add(item.Key, this.GetHotKeyFromString(item.Value));
            }
        }

        private void DrawContent(string content)
        {
            this.AddTextBox(content);
            this.AddButtons();
        }

        private void AddTextBox(string content)
        {
            this._TextBox.Top = 10;
            this._TextBox.Left = 64;
            this._TextBox.Width = this.Width - 64 - 32;
            this._TextBox.Height = this.Height - 64 - 32;
            this._TextBox.BackColor = Color.Azure;
            this._TextBox.Multiline = true;
            this._TextBox.ScrollBars = ScrollBars.Both;
            this._TextBox.Text = content;
            this._TextBox.ReadOnly = true;
            this._TextBox.HideSelection = true;
            this._TextBox.SelectionStart = 0;
            this._TextBox.SelectionLength = 0;
            this._TextBox.KeyUp += ScrollableMessageBox_KeyUp;
            this.Controls.Add(this._TextBox);
            this.Update();
        }

        private Button AddButton(ScrollableMsgBoxButtonType button, int height = 32, int width = 96)
        {
            string locale = this._Localizations.SingleOrDefault(v => v.Key == button).Value;
            Button result = new Button();
            result.Height = height;
            result.Width = width;
            result.Text = locale;
            result.UseMnemonic = true;
            result.Name = button.ToString();
            result.Click += Result_Click;
            result.KeyUp += ScrollableMessageBox_KeyUp;
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
                this.HideDialog();
            }
        }

        private void HideDialog()
        {
            this.Visible = false;
        }

        private void AddButtons()
        {
            switch (this._Buttons)
            {
                case MessageBoxButtons.OK:
                    this._ButtonControls.Add(this.AddButton(ScrollableMsgBoxButtonType.OkButton));
                    break;

                case MessageBoxButtons.OKCancel:
                    this._ButtonControls.Add(this.AddButton(ScrollableMsgBoxButtonType.OkButton));
                    this._ButtonControls.Add(this.AddButton(ScrollableMsgBoxButtonType.CancelButton));
                    break;

                case MessageBoxButtons.AbortRetryIgnore:
                    this._ButtonControls.Add(this.AddButton(ScrollableMsgBoxButtonType.AbortButton));
                    this._ButtonControls.Add(this.AddButton(ScrollableMsgBoxButtonType.RetryButton));
                    this._ButtonControls.Add(this.AddButton(ScrollableMsgBoxButtonType.IgnoreButton));
                    break;

                case MessageBoxButtons.YesNoCancel:
                    this._ButtonControls.Add(this.AddButton(ScrollableMsgBoxButtonType.YesButton));
                    this._ButtonControls.Add(this.AddButton(ScrollableMsgBoxButtonType.NoButton));
                    this._ButtonControls.Add(this.AddButton(ScrollableMsgBoxButtonType.CancelButton));
                    break;

                case MessageBoxButtons.YesNo:
                    this._ButtonControls.Add(this.AddButton(ScrollableMsgBoxButtonType.YesButton));
                    this._ButtonControls.Add(this.AddButton(ScrollableMsgBoxButtonType.NoButton));
                    break;

                case MessageBoxButtons.RetryCancel:
                    this._ButtonControls.Add(this.AddButton(ScrollableMsgBoxButtonType.RetryButton));
                    this._ButtonControls.Add(this.AddButton(ScrollableMsgBoxButtonType.CancelButton));
                    break;

                default:
                    break;
            }

            this.DrawButtons();
        }

        private void DrawButtons()
        {
            foreach (Button btn in this._ButtonControls)
            {
                int index = this._ButtonControls.IndexOf(btn) + 1;

                btn.Left = (64 * index) + (this._ButtonControls.IndexOf(btn) * btn.Width);

                btn.Top = this.Height - 48 - btn.Height;
                this.Controls.Add(btn);
                this.Update();
            }
        }

        private Keys GetHotKeyFromString(string value)
        {
            char hotkey;

            if (!string.IsNullOrWhiteSpace(value))
            {
                if (value.Contains("&"))
                {
                    int detectedHotKeyPrefix = 0;
                    foreach (char item in value)
                    {
                        if (item == '&')
                        {
                            detectedHotKeyPrefix = value.IndexOf(item);
                            if (detectedHotKeyPrefix < value.Length - 1)
                            {
                                hotkey = value[detectedHotKeyPrefix + 1];
                                return (Keys)char.ToUpper(hotkey);
                            }
                        }
                    }
                }
            }
            return Keys.None;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            KeyValuePair<Icon, MessageBoxIcon> selection = this._IconMapping.First(v => v.Value == this._Icon);
            e.Graphics.DrawIcon(selection.Key, 10, 10);
            base.OnPaint(e);
        }

        public void Dispose()
        {
            this.EventHandlers(false);
            foreach (Control item in this.Controls)
            {
                item.Dispose();
            }
            base.Dispose();
        }
    }
}