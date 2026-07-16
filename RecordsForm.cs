namespace IT04Form;

/// <summary>Lists every saved Person from the database and lets the user view the full uploaded image.</summary>
public class RecordsForm : Form
{
    private readonly DataGridView _grid = new();

    public RecordsForm()
    {
        Text = "Saved Records";
        ClientSize = new Size(1000, 560);
        StartPosition = FormStartPosition.CenterParent;
        BackColor = Color.White;
        Font = new Font("Segoe UI", 9F);

        List<Person> rows;
        try
        {
            rows = Db.GetAll();
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, "Failed to load records", MessageBoxButtons.OK, MessageBoxIcon.Error);
            rows = new List<Person>();
        }

        Controls.Add(BuildHeader(rows.Count));

        if (rows.Count > 0)
        {
            BuildGrid(rows);
            Controls.Add(_grid);
        }
        else
        {
            Controls.Add(new Label
            {
                Text = "No records yet.",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.Gray,
                Font = new Font("Segoe UI", 12F),
            });
        }
    }

    private Panel BuildHeader(int count)
    {
        var header = new Panel
        {
            Dock = DockStyle.Top,
            Height = 48,
            BackColor = Color.FromArgb(46, 125, 50),
        };

        var title = new Label
        {
            Text = $"Saved Records ({count})",
            Dock = DockStyle.Fill,
            BackColor = Color.Transparent,
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 12F, FontStyle.Bold),
            TextAlign = ContentAlignment.MiddleLeft,
            Padding = new Padding(16, 0, 0, 0),
        };

        var viewImageButton = new Button
        {
            Text = "View image",
            Dock = DockStyle.Right,
            Width = 110,
        };
        viewImageButton.Click += (_, _) =>
        {
            if (_grid.CurrentRow is null)
            {
                MessageBox.Show(this, "Select a record first.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            ShowImageForRow(_grid.CurrentRow.Index);
        };

        header.Controls.Add(title);
        header.Controls.Add(viewImageButton);
        return header;
    }

    private void BuildGrid(List<Person> rows)
    {
        _grid.Dock = DockStyle.Fill;
        _grid.ReadOnly = true;
        _grid.AllowUserToAddRows = false;
        _grid.AllowUserToDeleteRows = false;
        _grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        _grid.MultiSelect = false;
        _grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        _grid.BackgroundColor = Color.White;
        _grid.BorderStyle = BorderStyle.None;
        _grid.EnableHeadersVisualStyles = false;
        _grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(232, 245, 233);
        _grid.RowTemplate.Height = 44;

        _grid.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "Id",
            HeaderText = "ID",
            AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
            Width = 50,
        });

        var profileColumn = new DataGridViewImageColumn
        {
            Name = "Profile",
            HeaderText = "Profile",
            ImageLayout = DataGridViewImageCellLayout.Zoom,
            AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
            Width = 70,
        };
        profileColumn.DefaultCellStyle.NullValue = null;
        _grid.Columns.Add(profileColumn);

        _grid.Columns.Add("FirstName", "First Name");
        _grid.Columns.Add("LastName", "Last Name");
        _grid.Columns.Add("Email", "Email");
        _grid.Columns.Add("Phone", "Phone");
        _grid.Columns.Add("BirthDay", "Birth Day");
        _grid.Columns.Add("Occupation", "Occupation");
        _grid.Columns.Add("Sex", "Sex");
        _grid.Columns.Add("Created", "Created");

        foreach (var p in rows)
        {
            _grid.Rows.Add(
                p.Id,
                ImageUtil.FromDataUrl(p.ProfileBase64),
                p.FirstName,
                p.LastName,
                p.Email,
                p.Phone,
                p.BirthDay.ToString("dd/MM/yyyy"),
                p.Occupation,
                p.Sex,
                p.CreatedAt.ToString("yyyy-MM-dd HH:mm"));
        }

        _grid.CellDoubleClick += (_, e) =>
        {
            if (e.RowIndex >= 0)
            {
                ShowImageForRow(e.RowIndex);
            }
        };
    }

    private void ShowImageForRow(int rowIndex)
    {
        if (rowIndex < 0 || rowIndex >= _grid.Rows.Count)
        {
            return;
        }

        var row = _grid.Rows[rowIndex];
        if (row.Cells["Profile"].Value is not Image image)
        {
            MessageBox.Show(this, "This record has no viewable profile image.", "No Image", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var firstName = row.Cells["FirstName"].Value?.ToString() ?? "";
        var lastName = row.Cells["LastName"].Value?.ToString() ?? "";

        using var preview = new Form
        {
            Text = $"Profile - {firstName} {lastName}",
            StartPosition = FormStartPosition.CenterParent,
            ClientSize = CapSize(image.Size, 800, 800),
        };
        var pictureBox = new PictureBox
        {
            Dock = DockStyle.Fill,
            SizeMode = PictureBoxSizeMode.Zoom,
            Image = image,
        };
        preview.Controls.Add(pictureBox);
        preview.ShowDialog(this);

        // Detach before the preview/PictureBox get disposed so the grid's
        // thumbnail (the same Image instance) survives closing this dialog.
        pictureBox.Image = null;
    }

    private static Size CapSize(Size original, int maxWidth, int maxHeight)
    {
        if (original.Width <= 0 || original.Height <= 0)
        {
            return new Size(maxWidth / 2, maxHeight / 2);
        }

        double scale = Math.Min(1.0, Math.Min((double)maxWidth / original.Width, (double)maxHeight / original.Height));
        int width = Math.Min(maxWidth, Math.Max(200, (int)(original.Width * scale)));
        int height = Math.Min(maxHeight, Math.Max(200, (int)(original.Height * scale)));
        return new Size(width, height);
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        foreach (DataGridViewRow row in _grid.Rows)
        {
            if (row.Cells["Profile"].Value is Image image)
            {
                image.Dispose();
            }
        }
        base.OnFormClosed(e);
    }
}
