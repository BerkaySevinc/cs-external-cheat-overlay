using BekoS;
using Offsets;


using static Offsets.Offsets;

#nullable disable warnings

namespace CSGO_S
{


    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Control.CheckForIllegalCrossThreadCalls = false;
        }



        CSGO csgo = new CSGO(5);
        private async void Form1_Load(object sender, EventArgs e)
        {
            csgo.Updated += () =>
            {
                label2.Text = "Health: " + csgo.LocalPlayer.Health;
                label3.Text = "Armor: " + csgo.LocalPlayer.Armor;

                label4.Text = "Alive: " + csgo.LocalPlayer.IsAlive;
                label5.Text = "Team: " + csgo.LocalPlayer.Team;

                label6.Text = "Teammates: " + csgo.LocalTeam.Players.Count(p => p.IsAlive) + " / " + csgo.LocalTeam.Players.Count;
                label7.Text = "Enemies: " + csgo.EnemyTeam.Players.Count(p => p.IsAlive) + " / " + csgo.EnemyTeam.Players.Count;

                label9.Text = "X: " + csgo.LocalPlayer.X;
                label10.Text = "Y: " + csgo.LocalPlayer.Y;
                label11.Text = "Z: " + csgo.LocalPlayer.Z;

                csgo.Overlay.Reset();
                foreach (var player in csgo.LocalTeam.Players.Where(p => p.IsAlive && p.MemoryAddress != csgo.LocalPlayer.MemoryAddress))
                {
                    var rect = player.GetScreenRectangle(BekoS.Screen.Width, BekoS.Screen.Heigth);
                    if (rect is null) continue;

                    csgo.Overlay.DrawRectangle(Color.LimeGreen, (Rectangle)rect);
                }
                foreach (var player in csgo.EnemyTeam.Players.Where(p => p.IsAlive))
                {
                    var rect = player.GetScreenRectangle(BekoS.Screen.Width, BekoS.Screen.Heigth);
                    if (rect is null) continue;

                    csgo.Overlay.DrawRectangle(Color.Red, (Rectangle)rect);
                }
            };

            await csgo.WaitForOpenAsync(1000);

            label1.Text = "Connected: ✔";

            await csgo.Overlay.WaitForConnectAsync(1000);
        }




        /*
        private void beko_CheckBox_Tick1_CheckedChanged(object sender)
        {
            if (!csgo.Memory.IsConnected) return;

            bool isOn = ((Client.Beko_CheckBox_Tick)sender).Checked;

            if (isOn)
            {
                Task.Run(() =>
                {
                    int localTeamNumber = csgo.Memory.Read<int>($"client.dll+{dwLocalPlayer},{m_iTeamNum}", false);

                    // Loop Enemies
                    for (int i = 1; i <= 32; i++)
                    {
                        int entity = csgo.Memory.Read<int>($"client.dll+{dwEntityList + i * 0x10}", false);
                        if (entity == 0) break;

                        int teamNumber = csgo.Memory.Read<int>($"client.dll+{dwEntityList + i * 0x10},{m_iTeamNum}", false);
                        bool isSameTeam = localTeamNumber == teamNumber;

                        int health = csgo.Memory.Read<int>($"client.dll+{dwEntityList + i * 0x10},{m_iHealth}", false);
                    }

                    Thread.Sleep(1000);
                });
            }
        }
        */

    }






}