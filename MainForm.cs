namespace IT04Form;

/// <summary>
/// The IT 04-1 registration form. Entire UI is built in code (no Designer.cs/resx)
/// per project constraints -- everything lives in this one file.
/// </summary>
public class MainForm : Form
{
    private const int LeftX = 24;
    private const int RightX = 396;
    private const int FieldWidth = 340;
    private const int FullWidth = 712;

    private static readonly Color GreenColor = Color.FromArgb(46, 125, 50);
    private static readonly Color GreyColor = Color.FromArgb(158, 158, 158);
    private static readonly Color ErrorColor = Color.FromArgb(211, 47, 47);

    private readonly TextBox _txtFirstName;
    private readonly TextBox _txtLastName;
    private readonly TextBox _txtEmail;
    private readonly TextBox _txtPhone;
    private readonly PictureBox _picProfile;
    private readonly Label _lblProfileFile;
    private readonly DateTimePicker _dtpBirthDay;
    private readonly ComboBox _cboOccupation;
    private readonly Panel _pnlSex;
    private readonly Label _lblSuccess;
    private readonly Dictionary<string, Label> _errorLabels = new();

    private string _profileBase64 = "";

    public MainForm()
    {
        Text = "IT 04-1";
        ClientSize = new Size(760, 600);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;
        StartPosition = FormStartPosition.CenterScreen;
        BackColor = Color.White;
        Font = new Font("Segoe UI", 9F);
        AutoScaleMode = AutoScaleMode.None;

        // ---- Header ----
        var header = new Panel
        {
            Dock = DockStyle.Top,
            Height = 52,
            BackColor = GreenColor,
        };
        var lblTitle = new Label
        {
            Text = "IT 04-1",
            Font = new Font("Segoe UI", 14F, FontStyle.Bold),
            ForeColor = Color.White,
            BackColor = Color.Transparent,
            AutoSize = true,
            Location = new Point(16, 12),
        };
        var btnViewRecords = new Button
        {
            Text = "View saved records",
            Size = new Size(160, 32),
            // No Anchor: the form is fixed-size, and anchoring here would be
            // computed against the header panel's pre-dock default width (200),
            // which throws the button off-screen once the panel docks to 760.
            Location = new Point(584, 10),
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.White,
            ForeColor = GreenColor,
        };
        btnViewRecords.Click += (_, _) =>
        {
            using var records = new RecordsForm();
            records.ShowDialog(this);
        };
        header.Controls.Add(lblTitle);
        header.Controls.Add(btnViewRecords);
        Controls.Add(header);

        // ---- Success banner (hidden until a save succeeds) ----
        _lblSuccess = new Label
        {
            Location = new Point(LeftX, 58),
            Size = new Size(FullWidth, 24),
            Font = new Font("Segoe UI", 10F, FontStyle.Bold),
            ForeColor = GreenColor,
            BackColor = Color.Transparent,
            Visible = false,
        };
        Controls.Add(_lblSuccess);

        // ---- Simple text fields ----
        _txtFirstName = new TextBox();
        _txtLastName = new TextBox();
        _txtEmail = new TextBox();
        _txtPhone = new TextBox();

        // ---- Profile (Browse button + filename + 48x48 preview) ----
        var pnlProfile = new Panel { BackColor = Color.White };
        var btnBrowse = new Button
        {
            Text = "Browse",
            Location = new Point(0, 12),
            Size = new Size(90, 24),
            FlatStyle = FlatStyle.Flat,
        };
        _picProfile = new PictureBox
        {
            Location = new Point(100, 0),
            Size = new Size(48, 48),
            SizeMode = PictureBoxSizeMode.Zoom,
            BorderStyle = BorderStyle.FixedSingle,
        };
        _lblProfileFile = new Label
        {
            Location = new Point(156, 14),
            Size = new Size(184, 20),
            BackColor = Color.Transparent,
            AutoEllipsis = true,
        };
        btnBrowse.Click += (_, _) =>
        {
            using var dlg = new OpenFileDialog
            {
                Filter = "Image files|*.png;*.jpg;*.jpeg;*.gif;*.bmp;*.webp|All files|*.*",
            };
            if (dlg.ShowDialog(this) != DialogResult.OK) return;

            _profileBase64 = ImageUtil.ToDataUrl(dlg.FileName);
            _lblProfileFile.Text = Path.GetFileName(dlg.FileName);

            var oldImage = _picProfile.Image;
            _picProfile.Image = ImageUtil.FromDataUrl(_profileBase64);
            oldImage?.Dispose();
        };
        pnlProfile.Controls.Add(btnBrowse);
        pnlProfile.Controls.Add(_picProfile);
        pnlProfile.Controls.Add(_lblProfileFile);

        // ---- Birth Day ----
        _dtpBirthDay = new DateTimePicker
        {
            Format = DateTimePickerFormat.Custom,
            CustomFormat = "dd/MM/yyyy",
            MinDate = new DateTime(1900, 1, 1),
            MaxDate = DateTime.Today,
            ShowCheckBox = true,
            Checked = false,
        };

        // ---- Occupation ----
        _cboOccupation = new ComboBox
        {
            DropDownStyle = ComboBoxStyle.DropDownList,
        };
        _cboOccupation.Items.AddRange(Constants.Occupations);
        _cboOccupation.SelectedIndex = -1;

        // ---- Sex ----
        _pnlSex = new Panel { BackColor = Color.White };
        var sexX = 0;
        foreach (var sex in Constants.Sexes)
        {
            // AutoCheck = false on purpose: with the default (true), WinForms
            // selects a radio as soon as the group receives focus, which would
            // satisfy the required-Sex rule without the user ever choosing one.
            var rdo = new RadioButton
            {
                Text = sex,
                AutoSize = true,
                Location = new Point(sexX, 4),
                BackColor = Color.Transparent,
                AutoCheck = false,
            };
            rdo.Click += (sender, _) =>
            {
                foreach (Control control in _pnlSex.Controls)
                {
                    if (control is RadioButton radio)
                    {
                        radio.Checked = ReferenceEquals(radio, sender);
                    }
                }
            };
            _pnlSex.Controls.Add(rdo);
            sexX += 120;
        }

        // ---- Lay out the rows and collect the per-field error labels ----
        var errFirstName = AddRow(LeftX, 96, "First Name", _txtFirstName, FieldWidth, 24);
        var errLastName = AddRow(RightX, 96, "Last Name", _txtLastName, FieldWidth, 24);
        var errEmail = AddRow(LeftX, 172, "Email", _txtEmail, FieldWidth, 24);
        var errPhone = AddRow(RightX, 172, "Phone", _txtPhone, FieldWidth, 24);
        var errProfile = AddRow(LeftX, 248, "Profile", pnlProfile, FieldWidth, 48);
        var errBirthDay = AddRow(RightX, 248, "Birth Day", _dtpBirthDay, FieldWidth, 24);
        var errOccupation = AddRow(LeftX, 348, "Occupation", _cboOccupation, FullWidth, 24);
        var errSex = AddRow(LeftX, 424, "Sex", _pnlSex, FieldWidth, 28);

        _errorLabels[Constants.Fields.FirstName] = errFirstName;
        _errorLabels[Constants.Fields.LastName] = errLastName;
        _errorLabels[Constants.Fields.Email] = errEmail;
        _errorLabels[Constants.Fields.Phone] = errPhone;
        _errorLabels[Constants.Fields.ProfileBase64] = errProfile;
        _errorLabels[Constants.Fields.BirthDay] = errBirthDay;
        _errorLabels[Constants.Fields.Occupation] = errOccupation;
        _errorLabels[Constants.Fields.Sex] = errSex;

        // ---- Save / Clear ----
        var btnSave = new Button
        {
            Text = "Save",
            Location = new Point(LeftX, 504),
            Size = new Size(110, 36),
            BackColor = GreenColor,
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
        };
        btnSave.FlatAppearance.BorderSize = 0;

        var btnClear = new Button
        {
            Text = "Clear",
            Location = new Point(LeftX + 126, 504),
            Size = new Size(110, 36),
            BackColor = GreyColor,
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
        };
        btnClear.FlatAppearance.BorderSize = 0;

        btnSave.Click += (_, _) =>
        {
            var input = BuildInput();
            var errors = Validation.Validate(input);
            ClearErrors();

            if (errors.Count > 0)
            {
                foreach (var error in errors)
                {
                    if (_errorLabels.TryGetValue(error.Key, out var label))
                    {
                        label.Text = error.Value;
                    }
                }
                _lblSuccess.Visible = false;
                return;
            }

            int id;
            try
            {
                id = Db.Insert(input);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Save failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            _lblSuccess.Text = $"{Constants.Messages.SaveSuccess} !!!  ID: {id}";
            _lblSuccess.Visible = true;
            ResetForm();
        };

        btnClear.Click += (_, _) =>
        {
            ResetForm();
            _lblSuccess.Visible = false;
        };

        Controls.Add(btnSave);
        Controls.Add(btnClear);
    }

    /// <summary>
    /// Places a caption label, an input control, and an (initially empty) red error
    /// label directly under it. Returns the error label so the caller can register it
    /// in <see cref="_errorLabels"/>.
    /// </summary>
    private Label AddRow(int x, int y, string caption, Control input, int width, int height)
    {
        var cap = new Label
        {
            Text = caption,
            Location = new Point(x, y),
            AutoSize = true,
            BackColor = Color.Transparent,
        };
        Controls.Add(cap);

        input.Location = new Point(x, y + 18);
        input.Size = new Size(width, height);
        Controls.Add(input);

        var err = new Label
        {
            Location = new Point(x, y + 18 + height + 4),
            Size = new Size(width, 16),
            Font = new Font("Segoe UI", 8F),
            ForeColor = ErrorColor,
            BackColor = Color.Transparent,
        };
        Controls.Add(err);

        return err;
    }

    private PersonInput BuildInput() => new(
        _txtFirstName.Text,
        _txtLastName.Text,
        _txtEmail.Text,
        _txtPhone.Text,
        _dtpBirthDay.Checked ? _dtpBirthDay.Value.Date : (DateTime?)null,
        _cboOccupation.SelectedItem?.ToString() ?? "",
        GetSelectedSex(),
        _profileBase64);

    private string GetSelectedSex()
    {
        foreach (Control control in _pnlSex.Controls)
        {
            if (control is RadioButton { Checked: true } radio)
            {
                return radio.Text;
            }
        }
        return "";
    }

    private void ClearErrors()
    {
        foreach (var label in _errorLabels.Values)
        {
            label.Text = "";
        }
    }

    private void ResetForm()
    {
        _txtFirstName.Text = "";
        _txtLastName.Text = "";
        _txtEmail.Text = "";
        _txtPhone.Text = "";
        _dtpBirthDay.Checked = false;
        _cboOccupation.SelectedIndex = -1;

        foreach (Control control in _pnlSex.Controls)
        {
            if (control is RadioButton radio)
            {
                radio.Checked = false;
            }
        }

        var oldImage = _picProfile.Image;
        _picProfile.Image = null;
        oldImage?.Dispose();
        _lblProfileFile.Text = "";
        _profileBase64 = "";

        ClearErrors();
    }
}
