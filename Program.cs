using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Text;
using System.Net.Sockets;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;

namespace NeoManageWindowsForms
{
    public class MainPageForm : Form
    {
        private readonly WindowsFilePicker _windowsFilePicker = new WindowsFilePicker();

        private TextBox FileEntry;
        private TextBox SSIDEntry;
        private TextBox PasswordEntry;
        private TextBox ServerIPEntry;
        private TextBox ServerPortEntry;
        private Label StatusLabel;
        private List<UserProfile> _userProfiles = new List<UserProfile>();
        private ComboBox ProfileComboBox;
        private ProgressBar BatteryProgressBar;
        private Label BatteryPercentageLabel;

        /// <summary>
        /// Constructor for the main form.
        /// Initializes the main form and UI components.
        /// </summary>
        public MainPageForm()
        {
            InitializeComponents();
            InitializeProfiles(); // Initialize profile management UI components
        }

        /// <summary>
        /// Initializes the main UI components.
        /// </summary>
        private void InitializeComponents()
        {
            // Set initial properties for the main form
            Text = "Neo Manager";
            Width = 600;
            Height = 500;

            // Create and initialize UI components
            FileEntry = new TextBox
            {
                Width = 300,
                Top = 20,
                Left = 20
            };

            // "Browse" button for picking files
            Button browseButton = new Button
            {
                Text = "Browse",
                Width = 100,
                Top = 20,
                Left = 330
            };
            browseButton.Click += BrowseButton_Clicked;

            // Label and textbox for new SSID
            Label ssidLabel = new Label
            {
                Text = "New SSID:",
                Top = 64,
                Left = 20,
                Width = 80,
                TextAlign = ContentAlignment.MiddleRight
            };
            SSIDEntry = new TextBox
            {
                Width = 150,
                Top = 60,
                Left = 110
            };

            // Label and textbox for new password
            Label passwordLabel = new Label
            {
                Text = "New Password:",
                Top = 100,
                Left = 20,
                Width = 90,
                TextAlign = ContentAlignment.MiddleLeft
            };
            PasswordEntry = new TextBox
            {
                Width = 150,
                Top = 100,
                Left = 110,
                UseSystemPasswordChar = true
            };

            // "Update" button to update secrets
            Button updateButton = new Button
            {
                Text = "Update",
                Width = 100,
                Top = 140,
                Left = 20
            };
            updateButton.Click += UpdateButton_Clicked;

            // "Update All Drives" button to update secrets on all drives
            Button updateAllButton = new Button
            {
                Text = "Update All Drives",
                Width = 150,
                Top = 140,
                Left = 140
            };
            updateAllButton.Click += UpdateAllButton_Clicked;

            // Label and textbox for server IP
            Label serverIPLabel = new Label
            {
                Text = "Server IP:",
                Top = 180,
                Left = 20,
                Width = 80,
                TextAlign = ContentAlignment.MiddleRight
            };
            ServerIPEntry = new TextBox
            {
                Width = 150,
                Top = 180,
                Left = 110
            };

            // Label and textbox for server port
            Label serverPortLabel = new Label
            {
                Text = "Server Port:",
                Top = 220,
                Left = 20,
                Width = 80,
                TextAlign = ContentAlignment.MiddleRight
            };
            ServerPortEntry = new TextBox
            {
                Width = 60,
                Top = 220,
                Left = 110
            };

            // "Get Battery" button to retrieve battery info
            Button batteryButton = new Button
            {
                Text = "Get Battery",
                Width = 100,
                Top = 260,
                Left = 20
            };
            batteryButton.Click += BatteryButton_Clicked;

            // Status label to display messages
            StatusLabel = new Label
            {
                AutoSize = true,
                Top = 300,
                Left = 20
            };
            
            //Battery bar and label to show percentage
            BatteryProgressBar = new ProgressBar
            {
                Minimum = 0,
                Maximum = 100,
                Value = 0,
                Top = 260,
                Left = 130,
                Width = 200
            };

            BatteryPercentageLabel = new Label
            {
                Text = "0%",
                Top = 260,
                Left = 340,
                Width = 50,
                TextAlign = ContentAlignment.MiddleLeft
            };

            // Add UI components to the form
            Controls.Add(FileEntry);
            Controls.Add(browseButton);
            Controls.Add(ssidLabel);
            Controls.Add(passwordLabel);
            Controls.Add(SSIDEntry);
            Controls.Add(PasswordEntry);
            Controls.Add(updateButton);
            Controls.Add(updateAllButton);
            Controls.Add(serverIPLabel);
            Controls.Add(serverPortLabel);
            Controls.Add(ServerIPEntry);
            Controls.Add(ServerPortEntry);
            Controls.Add(batteryButton);
            Controls.Add(StatusLabel);
            Controls.Add(BatteryProgressBar);
            Controls.Add(BatteryPercentageLabel);

            // Create and configure the TableLayoutPanel for layout control
            TableLayoutPanel layoutPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20),
                ColumnCount = 2,
                RowCount = 7,
                ColumnStyles =
        {
            new ColumnStyle(SizeType.Absolute, 100), // Labels column
            new ColumnStyle(SizeType.Percent, 100)   // Textboxes column
        },
                RowStyles =
        {
            new RowStyle(SizeType.Absolute, 40),
            new RowStyle(SizeType.Absolute, 40),
            new RowStyle(SizeType.Absolute, 40),
            new RowStyle(SizeType.Absolute, 40),
            new RowStyle(SizeType.Absolute, 40),
            new RowStyle(SizeType.Absolute, 40),
            new RowStyle(SizeType.Percent, 100)
        }
            };

            // Add UI components to the layout panel
            layoutPanel.Controls.Add(ssidLabel, 0, 1);
            layoutPanel.Controls.Add(SSIDEntry, 1, 1);
            layoutPanel.Controls.Add(passwordLabel, 0, 2);
            layoutPanel.Controls.Add(PasswordEntry, 1, 2);
            layoutPanel.Controls.Add(updateButton, 0, 3);
            layoutPanel.Controls.Add(updateAllButton, 1, 3);
            layoutPanel.Controls.Add(serverIPLabel, 0, 4);
            layoutPanel.Controls.Add(ServerIPEntry, 1, 4);
            layoutPanel.Controls.Add(serverPortLabel, 0, 5);
            layoutPanel.Controls.Add(ServerPortEntry, 1, 5);
            layoutPanel.Controls.Add(batteryButton, 0, 6);
            layoutPanel.Controls.Add(StatusLabel, 0, 7);

            // Add the layout panel to the form's Controls collection
            Controls.Add(layoutPanel);
        }


        /// <summary>
        /// Initializes the UI components for profile management.
        /// </summary>
        private void InitializeProfiles()
        {
            ProfileComboBox = new ComboBox
            {
                Width = 150,
                Top = 260,
                Left = 140
            };

            Label profileLabel = new Label
            {
                Text = "Profiles:",
                Top = 260,
                Left = 20,
                Width = 60,
                TextAlign = ContentAlignment.MiddleRight
            };

            Button saveProfileButton = new Button
            {
                Text = "Save Profile",
                Width = 100,
                Top = 300,
                Left = 20
            };
            saveProfileButton.Click += SaveProfileButton_Clicked;

            Button loadProfileButton = new Button
            {
                Text = "Load Profile",
                Width = 100,
                Top = 300,
                Left = 140
            };
            loadProfileButton.Click += LoadProfileButton_Clicked;

            Controls.Add(profileLabel);
            Controls.Add(ProfileComboBox);
            Controls.Add(saveProfileButton);
            Controls.Add(loadProfileButton);

            // Create and configure the FlowLayoutPanel for profile management
            FlowLayoutPanel profilePanel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.LeftToRight,
                Dock = DockStyle.Bottom,
                Padding = new Padding(20)
            };

            profilePanel.Controls.Add(profileLabel);
            profilePanel.Controls.Add(ProfileComboBox);
            profilePanel.Controls.Add(saveProfileButton);
            profilePanel.Controls.Add(loadProfileButton);

            // Add the profilePanel to the form's Controls collection
            Controls.Add(profilePanel);
        }


        /// <summary>
        /// Event handler for the "Browse" button click.
        /// Opens a file picker dialog and updates the FileEntry textbox with the selected file path.
        /// </summary>
        private void BrowseButton_Clicked(object sender, EventArgs e)
        {
            string filePath = _windowsFilePicker.PickAFile();
            if (!string.IsNullOrEmpty(filePath))
            {
                FileEntry.Text = filePath;
            }
        }

        /// <summary>
        /// Event handler for the "Update" button click.
        /// Updates secrets (SSID and password) in the selected file.
        /// </summary>
        private void UpdateButton_Clicked(object sender, EventArgs e)
        {
            string filePath = FileEntry.Text;
            string newSSID = SSIDEntry.Text;
            string newPassword = PasswordEntry.Text;
            UpdateSecrets(filePath, newSSID, newPassword);
        }

        /// <summary>
        /// Event handler for the "Update All Drives" button click.
        /// Updates secrets (SSID and password) in all 'secrets.py' files on all drives.
        /// </summary>
        private async void UpdateAllButton_Clicked(object sender, EventArgs e)
        {
            string newSSID = SSIDEntry.Text;
            string newPassword = PasswordEntry.Text;
            await UpdateSecretsOnAllDrives(newSSID, newPassword);
        }

        /// <summary>
        /// Update secrets (SSID and password) in the specified file.
        /// </summary>
        /// <param name="filePath">Path of the file to be updated.</param>
        /// <param name="newSSID">New SSID to be set.</param>
        /// <param name="newPassword">New password to be set.</param>
        private void UpdateSecrets(string filePath, string newSSID, string newPassword)
        {
            try
            {
                // Read file contents
                string contents = File.ReadAllText(filePath);
                // Encryption key for AES encryption
                byte[] encryptionKey = new byte[] { 0x, 0x, 0x, 0x, 0x, 0x, 0x, 0x, 0x, 0x, 0x, 0x, 0x, 0x, 0x, 0x }; //32-bit key


                using (Aes aesAlg = Aes.Create())
                {
                    aesAlg.Key = encryptionKey;
                    byte[] passwordBytes = Encoding.UTF8.GetBytes(newPassword);
                    int paddingLength = 16 - (passwordBytes.Length % 16);
                    Array.Resize(ref passwordBytes, passwordBytes.Length + paddingLength);
                    for (int i = passwordBytes.Length - paddingLength; i < passwordBytes.Length; i++)
                    {
                        passwordBytes[i] = (byte)paddingLength;
                    }

                    aesAlg.Key = encryptionKey;
                    aesAlg.Mode = CipherMode.ECB; // Use ECB mode
                    aesAlg.Padding = PaddingMode.None; // No padding needed since we manually added padding

                    ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                    using (MemoryStream msEncrypt = new MemoryStream())
                    {
                        using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                        {
                            csEncrypt.Write(passwordBytes, 0, passwordBytes.Length);
                            csEncrypt.FlushFinalBlock();
                        }
                        byte[] encryptedPassword = msEncrypt.ToArray();

                        string encryptedPasswordPythonFormat = BitConverter.ToString(encryptedPassword).Replace("-", "\\x");

                        // Replace SSID and encrypted password in the contents
                        //contents = Regex.Replace(contents, @"'ssid'\s*:\s*'.*?'", $"'ssid': '{newSSID}'");
                        //contents = Regex.Replace(contents, @"'password'\s*:\s*'.*?'", $"'password': {BitConverter.ToString(encryptedPassword).Replace("-", ",")}");
                        //Console.WriteLine(BitConverter.ToString(encryptedPassword).Replace("-", ","));
                        // Write updated contents back to the file
                        //File.WriteAllText(filePath, contents);
                        // Create a new secrets dictionary with updated SSID and password
                        Dictionary<string, string> secrets = new Dictionary<string, string>
                        {
                            { "'ssid'", $"'{newSSID}'" },
                            { "'password'", $"b'\\x{encryptedPasswordPythonFormat}'" },  //BitConverter.ToString(encryptedPassword).Replace("-", "\\x")
                            { "'timezone'", "\"America/Chicago\"" },
                            { "'listen_port'", "80" },
                            { "'imas_api'", "'https://'" },
                        };

                        Console.WriteLine(encryptedPasswordPythonFormat);

                        // Serialize the secrets dictionary to a C# string
                        string formattedSecrets = string.Join(",\n", secrets.Select(kv => $"{kv.Key} : {kv.Value}"));

                        // Write the C# script to a new file
                        //string cSharpCode = $"secrets = {{\n{formattedSecrets}\n}}";
                        //using (StreamWriter writer = new StreamWriter(filePath))
                        //{
                        //    writer.Write(cSharpCode);
                        //}
                        // Write the C# script to a temporary file
                        // the code creates a temporary file with the updated secrets content, then deletes the 
                        // original file, and finally renames the temporary file to the original file's name 
                        // to effectively update the contents of the original file without corrupting it. This approach 
                        // helps prevent issues that might occur when directly writing to the original file, ensuring safer file updates.
                        string tempFilePath = Path.Combine(Path.GetDirectoryName(filePath), "temp_secrets.cs");
                        string cSharpCode = $"secrets = {{\n{formattedSecrets}\n}}";
                        File.WriteAllText(tempFilePath, cSharpCode);
                                                                            
                        // Delete the original file and rename the temporary file
                        File.Delete(filePath);
                        File.Move(tempFilePath, filePath);

                        //string encryptedPasswordPythonFormat = BitConverter.ToString(encryptedPassword).Replace("-", "\\x");

                        // Replace SSID and encrypted password in the contents
                        //contents = Regex.Replace(contents, @"'ssid'\s*:\s*'.*?'", $"'ssid': '{newSSID}'");
                        //contents = Regex.Replace(contents, @"'password'\s*:\s*'.*?'", $"'password': b\\'{encryptedPasswordPythonFormat}'");

                        // Write updated contents back to the file
                        //File.WriteAllText(filePath, contents);


                        // Display success status
                        StatusLabel.Text = "SSID and password updated in the file successfully.";
                        StatusLabel.ForeColor = System.Drawing.Color.Green;
                    }
                }
            }
            catch (Exception ex)
            {
                // Display error status
                StatusLabel.Text = "An error occurred: " + ex.Message;
                StatusLabel.ForeColor = System.Drawing.Color.Red;
            }
        }

        /// <summary>
        /// Update secrets (SSID and password) in all 'secrets.py' files on all drives.
        /// </summary>
        /// <param name="newSSID">New SSID to be set.</param>
        /// <param name="newPassword">New password to be set.</param>
        private async Task UpdateSecretsOnAllDrives(string newSSID, string newPassword)
        {
            try
            {
                // Get the list of logical drives
                string[] drives = Directory.GetLogicalDrives();
                foreach (string drive in drives)
                {
                    if (drive != Path.GetPathRoot(Environment.GetFolderPath(Environment.SpecialFolder.System)))
                    {
                        // Update secrets for each drive asynchronously
                        await UpdateDriveSecrets(drive, newSSID, newPassword);
                    }
                }
            }
            catch (Exception ex)
            {
                // Display error status
                StatusLabel.Text = "An error occurred: " + ex.Message;
                StatusLabel.ForeColor = System.Drawing.Color.Red;
            }
        }

        /// <summary>
        /// Update secrets (SSID and password) in 'secrets.py' files within a specific drive.
        /// </summary>
        /// <param name="drive">Drive to search for secrets files.</param>
        /// <param name="newSSID">New SSID to be set.</param>
        /// <param name="newPassword">New password to be set.</param>
        private async Task UpdateDriveSecrets(string drive, string newSSID, string newPassword)
        {
            try
            {
                // Get all 'secrets.py' files within the specified drive
                string[] secretsFiles = Directory.GetFiles(drive, "secrets.py", SearchOption.AllDirectories);
                foreach (string secretsFile in secretsFiles)
                {
                    // Update secrets in each 'secrets.py' file and delay for better visualization
                    UpdateSecrets(secretsFile, newSSID, newPassword);
                    await Task.Delay(100);
                }
            }
            catch (Exception ex)
            {
                // Display error status
                StatusLabel.Text = "An error occurred: " + ex.Message;
                StatusLabel.ForeColor = System.Drawing.Color.Red;
            }
        }

        /// <summary>
        /// Event handler for the "Get Battery" button click.
        /// Sends a battery command to a server using TCP and displays the response.
        /// </summary>
        private void BatteryButton_Clicked(object sender, EventArgs e)
        {
            string serverIP = ServerIPEntry.Text;
            int serverPort;

            // Parse and validate the server port
            if (!int.TryParse(ServerPortEntry.Text, out serverPort))
            {
                StatusLabel.Text = "Invalid port number";
                StatusLabel.ForeColor = System.Drawing.Color.Red;
                return;
            }

            byte[] batteryHex = new byte[] { 0x00 };

            // Start a new thread for sending the battery command
            new Thread(() =>
            {
                try
                {
                    using (TcpClient client = new TcpClient(serverIP, serverPort))
                    using (NetworkStream stream = client.GetStream())
                    {
                        string batteryHexStr = BitConverter.ToString(batteryHex).Replace("-", "");
                        byte[] batteryCommand = new byte[batteryHexStr.Length / 2];
                        for (int i = 0; i < batteryHexStr.Length; i += 2)
                        {
                            batteryCommand[i / 2] = Convert.ToByte(batteryHexStr.Substring(i, 2), 16);
                        }

                        // Send the battery command to the server
                        stream.Write(batteryCommand, 0, batteryCommand.Length);
                        stream.Flush();

                        while (true)
                        {
                            byte[] buffer = new byte[1024];
                            int bytesRead;

                            do
                            {
                                bytesRead = stream.Read(buffer, 0, buffer.Length);
                            } while (bytesRead == 0);
                            if (bytesRead != -1)
                            {
                                double receivedDecimal = BytesToDouble(buffer);
                                int batteryPercentage = (int)Math.Round(receivedDecimal);
                                string result = string.Format("{0:F2}", receivedDecimal);
                                // Display response status
                                if (InvokeRequired)
                                {
                                    BeginInvoke(new Action(() =>
                                    {
                                        StatusLabel.Text = "Battery command sent: " + result + "%";
                                        StatusLabel.ForeColor = System.Drawing.Color.Blue;
                                        // Update the battery bar and percentage label
                                        BatteryProgressBar.Value = batteryPercentage;
                                        BatteryPercentageLabel.Text = batteryPercentage + "%";
                                        BatteryPercentageLabel.ForeColor = System.Drawing.Color.Green;
                                    }));
                                }
                                else
                                {
                                    StatusLabel.Text = "Battery command sent: " + result + "%";
                                    StatusLabel.ForeColor = System.Drawing.Color.Blue;
                                    // Update the battery bar and percentage label
                                    BatteryProgressBar.Value = batteryPercentage;
                                    BatteryPercentageLabel.Text = batteryPercentage + "%";
                                    BatteryPercentageLabel.ForeColor = System.Drawing.Color.Green;
                                }
                                break;
                            }
                            Thread.Sleep(1000);
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Display error status
                    Console.WriteLine("An error occurred: " + ex.Message);
                    StatusLabel.Text = "An error occurred: " + ex.Message;
                    StatusLabel.ForeColor = System.Drawing.Color.Red;
                }
            }).Start();
        }

        // Helper function to convert bytes to double
        private double BytesToDouble(byte[] bytes)
        {
            long longBits = 0;
            for (int i = 0; i < 8; i++)
            {
                longBits |= ((long)bytes[i] & 0xff) << (8 * i);
            }
            return BitConverter.Int64BitsToDouble(longBits);
        }

        /// <summary>
        /// Event handler for the "Save Profile" button click.
        /// Saves the current configuration as a user profile.
        /// </summary>
        private void SaveProfileButton_Clicked(object sender, EventArgs e)
        {
            // Prompt the user to enter a name for the profile
            string profileName = InputDialog.Show("Enter a name for the profile:", "Save Profile");

            // Check if a profile name was provided and it's not already in the list
            if (!string.IsNullOrEmpty(profileName) && ProfileComboBox.Items.IndexOf(profileName) == -1)
            {
                UserProfile userProfile = new UserProfile
                {
                    ProfileName = profileName,
                    FilePath = FileEntry.Text,
                    SSID = SSIDEntry.Text,
                    Password = PasswordEntry.Text,
                    ServerIP = ServerIPEntry.Text,
                    ServerPort = ServerPortEntry.Text
                };
                _userProfiles.Add(userProfile);
                ProfileComboBox.Items.Add(profileName);
            }
        }


        /// <summary>
        /// Event handler for the "Load Profile" button click.
        /// Loads a selected user profile and updates the UI components.
        /// </summary>
        private void LoadProfileButton_Clicked(object sender, EventArgs e)
        {
            // Load a selected user profile and update the UI components
            string selectedProfile = ProfileComboBox.SelectedItem?.ToString();
            if (!string.IsNullOrEmpty(selectedProfile))
            {
                UserProfile userProfile = _userProfiles.Find(profile => profile.ProfileName == selectedProfile);
                if (userProfile != null)
                {
                    FileEntry.Text = userProfile.FilePath;
                    SSIDEntry.Text = userProfile.SSID;
                    PasswordEntry.Text = userProfile.Password;
                    ServerIPEntry.Text = userProfile.ServerIP;
                    ServerPortEntry.Text = userProfile.ServerPort;
                }
            }
        }
    }



    public class WindowsFilePicker
    {
        /// <summary>
        /// Displays a file picker dialog and returns the selected file path.
        /// </summary>
        /// <returns>The selected file path or null if canceled.</returns>
        public string PickAFile()
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Python files (*.py)|*.py|All files (*.*)|*.*";
                openFileDialog.Title = "Pick a Python file";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    return openFileDialog.FileName;
                }
            }

            return null;
        }
    }

    /// <summary>
    /// Class to store user profiles for serialization.
    /// </summary>
    [Serializable]
    public class UserProfile
    {
        public string ProfileName { get; set; }
        public string FilePath { get; set; }
        public string SSID { get; set; }
        public string Password { get; set; }
        public string ServerIP { get; set; }
        public string ServerPort { get; set; }
    }

    // Main entry point of the application
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainPageForm());
        }
    }

    /// <summary>
    /// A utility class that displays an input dialog with a text box and a confirmation button.
    /// Allows users to input text and returns the entered text when confirmed.
    /// </summary>
    public class InputDialog
    {
        /// <summary>
        /// Displays an input dialog with the specified text and caption.
        /// </summary>
        /// <param name="text">The text to display in the dialog.</param>
        /// <param name="caption">The caption or title of the dialog.</param>
        /// <returns>The entered text if confirmed, or an empty string if canceled.</returns>
        public static string Show(string text, string caption)
        {
            Form prompt = new Form()
            {
                Width = 300,
                Height = 150,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = caption,
                StartPosition = FormStartPosition.CenterScreen
            };
            Label textLabel = new Label() { Left = 50, Top = 20, Text = text };
            TextBox textBox = new TextBox() { Left = 50, Top = 50, Width = 200 };
            Button confirmation = new Button() { Text = "Ok", Left = 150, Width = 100, Top = 70, DialogResult = DialogResult.OK };
            confirmation.Click += (sender, e) => { prompt.Close(); };
            prompt.Controls.Add(confirmation);
            prompt.Controls.Add(textLabel);
            prompt.Controls.Add(textBox);
            prompt.AcceptButton = confirmation;

            return prompt.ShowDialog() == DialogResult.OK ? textBox.Text : "";
        }
    }
}
