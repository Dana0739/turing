using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Text.RegularExpressions;
using Microsoft.Win32;
using System.IO;

namespace turing
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public int stateRows;
        public int stateCols;
        public int curpos;
        public int step;
        public int maxstep;
        public string curstate;
        public string filepath;
        public bool check;
        public bool on_turing;
        public string[,] states;
        public string strword;
        public List<string> tword;
        public TextBox[,] box;
        public MainWindow()
        {
            InitializeComponent();
            tword = new List<string>() { };
            on_turing = false;
            stateRows = 2;
            stateCols = 3;
            strword = "";
            check = false;
            curpos = 1;
            curstate = "q1";
            step = 0;
            maxstep = 1000;
            filepath = "";
            states = new string[,] { { "", "S0", "0", "1", "2"}, { "q1", "", "", "", ""}, { "q2", "", "", "", "" }, { "q3", "", "", "", "" }, { "q4", "", "", "", "" } };
            box = new TextBox[,] { { t1, t2, t3, t4}, { t5, t6, t7, t8}, { t9, t10, t11, t12}, {t13, t14, t15, t16 } };
        }

        private void New_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < stateCols - 1; i++)
            {
                for (int j = 0; j < stateRows - 1; j++)
                {
                    box[j, i].Text = "";
                    box[j, i].Background = Brushes.MintCream;
                }
            }
            states = new string[,] { { "", "S0", "0", "1", "2"}, { "q1", "", "", "", ""}, { "q2", "", "", "", "" }, { "q3", "", "", "", "" }, { "q4", "", "", "", "" } };
            check = false;
            strword = "";
            Word.Text = "";
            step = 0;
            maxstep = 1000;
            curpos = 1;
            curstate = "q1";
            tword = new List<string>() { };
            Word.Background = Brushes.MintCream;
            Unlock_State();
            while (Result.Children.Count > 0)
            {
                Result.Children.RemoveAt(Result.Children.Count - 1);
            }
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            if (stateRows < 5)
            {
                stateRows += 1;
                state.RowDefinitions.ElementAt(stateRows - 1).Height = state.RowDefinitions.ElementAt(0).Height;
            }
        }

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            if (stateRows > 2)
            {
                stateRows -= 1;
                GridLength length = new GridLength(0);
                state.RowDefinitions.ElementAt(stateRows).Height = length;
            }
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            if (Select_Path_Open())
            {
                string ftext = File.ReadAllText(filepath);
                string r = @"^(1|2|3|4|5){2}(0|1q(0|1|2|3|4)(0|1|2)(l|c|r)){16}S0?\s{0,}?((0|1|2)\s{0,}?){2,}S?\s{0,}?0?$";
                Regex regex = new Regex(r, RegexOptions.IgnoreCase);
                if (regex.IsMatch(ftext)) {
                    int newr = 0;
                    Int32.TryParse(ftext[0].ToString(), out newr);
                    if (stateRows < newr)
                    {
                        for (int i = 0; i <= (newr - stateRows); i++)
                        {
                            stateRows += 1;
                            state.RowDefinitions.ElementAt(stateRows - 1).Height = state.RowDefinitions.ElementAt(0).Height;
                        }
                    }
                    if (stateRows > newr)
                    {
                        for (int i = 0; i <= (stateRows - newr); i++)
                        {
                            stateRows -= 1;
                            GridLength length = new GridLength(0);
                            state.RowDefinitions.ElementAt(stateRows).Height = length;
                        }
                    }
                    int newc = 0;
                    Int32.TryParse(ftext[1].ToString(), out newc);
                    if (stateCols < newc)
                    {
                        for (int i = 0; i <= (newc - stateCols); i++)
                        {
                            stateCols += 1;
                            state.ColumnDefinitions.ElementAt(stateCols - 1).Width = state.ColumnDefinitions.ElementAt(0).Width;
                        }
                    }
                    if (stateCols > newc)
                    {
                        for (int i = 0; i <= (stateCols - newc); i++)
                        {
                            stateCols -= 1;
                            GridLength length = new GridLength(0);
                            state.ColumnDefinitions.ElementAt(stateCols).Width = length;
                        }
                    }
                    ftext = ftext.Remove(0, 2);
                    for (int i = 0; i < 4; i++)
                    {
                        for (int j = 0; j < 4; j++)
                        {
                            if (ftext.Substring(0, 1) == "1")
                            {
                                box[i, j].Text = ftext.Substring(1, 4);
                                ftext = ftext.Remove(0, 5);
                            }
                            else ftext = ftext.Remove(0, 1);
                        }
                    }
                    Word.Text = ftext;
                }
            }
            else
            {
                MessageBox.Show("Provided unsuitable file: " + filepath);
            }
        }

        private bool Select_Path_Open()
        {
            OpenFileDialog myDialog = new OpenFileDialog();
            myDialog.DefaultExt = ".txt";
            myDialog.Filter = "Text documents (*.txt)|*.txt";
            myDialog.CheckFileExists = true;
            myDialog.Multiselect = true;
            bool res = myDialog.ShowDialog() == true;
            if (res)
            {
                filepath = myDialog.FileName;
            }
            return res;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            Check_States();
            if (check)
            {
                if (Select_Path_Save())
                {
                    string data = stateRows.ToString() + stateCols.ToString();
                    for (int i = 0; i < 4; i++)
                    {
                        for (int j = 0; j < 4; j++)
                        {
                            if (box[i, j].Text == "") data += "0";
                            else
                            {
                                data += "1" + Regex.Replace(box[i, j].Text, @"\s+", "");
                            }
                        }
                    }
                    data += Word.Text;
                    File.WriteAllText(filepath, data);
                }
            }
            check = false;
        }

        public bool Select_Path_Save()
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.FileName = "SampleTuring";
            dlg.DefaultExt = ".txt";
            dlg.Filter = "Text documents (.txt)|*.txt";
            bool result = dlg.ShowDialog() == true;
            if (result)
            {
                filepath = dlg.FileName;
            }
            return result;
        }

        private void More_Click(object sender, RoutedEventArgs e)
        {
            if (stateCols < 5)
            {
                stateCols += 1;
                state.ColumnDefinitions.ElementAt(stateCols - 1).Width = state.ColumnDefinitions.ElementAt(0).Width;
            }
        }

        private void Less_Click(object sender, RoutedEventArgs e)
        {
            if (stateCols > 3)
            {
                stateCols -= 1;
                GridLength length = new GridLength(0);
                state.ColumnDefinitions.ElementAt(stateCols).Width = length;
            }
        }

        private void Lock_State()
        {
            on_turing = true;
            TextBox[] b = new TextBox[] { t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15, t16 };
            foreach (TextBox t in b)
            {
                t.IsEnabled = false;
            }
            AddState.IsEnabled = false;
            RemoveState.IsEnabled = false;
            Less.IsEnabled = false;
            More.IsEnabled = false;
            Open.IsEnabled = false;
            Word.IsEnabled = false;
        }

        private void Unlock_State()
        {
            on_turing = false;
            TextBox[] b = new TextBox[] { t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15, t16 };
            foreach (TextBox t in b)
            {
                t.IsEnabled = true;
            }
            AddState.IsEnabled = true;
            RemoveState.IsEnabled = true;
            Less.IsEnabled = true;
            More.IsEnabled = true;
            Open.IsEnabled = true;
            Word.IsEnabled = true;
        }

        private void Step_Click(object sender, RoutedEventArgs e)
        {   
            if (!on_turing)
            {
                Check_States();
                if (check)
                {
                    Lock_State();
                    Prepare();
                    OutputState();
                    Turing_Step();
                }
            }
            else Turing_Step();
        }

        private void ToEnd_Click(object sender, RoutedEventArgs e)
        {
            if (!on_turing)
            {
                Check_States();
                if (check)
                {
                    Lock_State();
                    Prepare();
                    OutputState();
                    Turing_Loop();
                }
            }
            else Turing_Loop();
        }

        private void Turing_Loop()
        {
            while (step < maxstep && curstate != "q0")
            {
                Turing_Step();
            }
        }

        private void Turing_Step()
        {
            for (int k = 0; k < 4; k++)
            {
                for (int l = 0; l < 4; l++)
                {
                    box[k, l].Background = Brushes.MintCream;
                }
            }
            if (step >= maxstep) return;
            int i = 0;
            Int32.TryParse(curstate[1].ToString(), out i);
            if (i > 0)
            {
                int j = 0;
                if (tword[curpos][0].ToString().ToLower() == "s") j = 1;
                else
                {
                    Int32.TryParse(tword[curpos].ToString(), out j);
                    j += 2;
                }
                string command = states[i, j];
                box[i - 1, j - 1].Background = Brushes.LightPink;
                if (command != "")
                {
                    curstate = command[0].ToString() + command[1].ToString();
                    tword[curpos] = command[2].ToString();
                    if (command[3].ToString().ToLower() == "r")
                    {
                        if (curpos == tword.Count() - 1) tword.Add("s");
                        curpos++;
                    }
                    else if (command[3].ToString().ToLower() == "l")
                    {
                        if (curpos == 0)
                        {
                            tword.Insert(0, "s");
                            curpos++;
                        }
                        curpos--;
                    }
                    step++;
                    OutputState();
                }
                else curstate = "q0";
            }
        }

        private void OutputState()
        {
            Grid g = new Grid();
            g.Margin = up.Margin;
            g.Background = Brushes.MintCream;
            RowDefinition r = new RowDefinition();
            g.RowDefinitions.Add(r);
            ColumnDefinition c = new ColumnDefinition();
            GridLength l = new GridLength(50);
            c.Width = l;
            g.ColumnDefinitions.Add(c);
            for (int i = 0; i < tword.Count(); i++ )
            {
                c = new ColumnDefinition();
                c.Width = state.ColumnDefinitions.ElementAt(0).Width;
                g.ColumnDefinitions.Add(c);
            }
            Result.Children.Add(g);
            TextBlock t = new TextBlock();
            t.Text = curstate;
            t.FontSize = 17;
            t.TextAlignment = TextAlignment.Left;
            t.Background = Brushes.LightPink;
            Grid.SetColumn(t, curpos + 1);
            Grid.SetRow(t, 0);
            g.Children.Add(t);

            g = new Grid();
            g.Margin = down.Margin;
            g.Background = Brushes.MintCream;
            r = new RowDefinition();
            g.RowDefinitions.Add(r);
            c = new ColumnDefinition();
            l = new GridLength(50);
            c.Width = l;
            g.ColumnDefinitions.Add(c);
            t = new TextBlock();
            t.Text = "p" + step.ToString() + "=";
            t.FontSize = 17;
            t.TextAlignment = TextAlignment.Left;
            Grid.SetColumn(t, 0);
            Grid.SetRow(t, 0);
            g.Children.Add(t);
            for (int i = 0; i < tword.Count(); i++)
            {
                c = new ColumnDefinition();
                c.Width = state.ColumnDefinitions.ElementAt(0).Width;
                g.ColumnDefinitions.Add(c);
                t = new TextBlock();
                t.Text = tword[i];
                t.FontSize = 17;
                t.TextAlignment = TextAlignment.Left;
                Grid.SetColumn(t, i + 1);
                Grid.SetRow(t, 0);
                if (i == curpos) t.Background = Brushes.LightPink;
                g.Children.Add(t);
            }
            Result.Children.Add(g);
        }

        private void Prepare()
        {
            foreach (char c in strword)
            {
                tword.Add(c.ToString());
            }
            if (tword[0].ToLower() == "s" && tword[1] == "0" && tword[2] == " ")
            {
                tword[0] = "s0";
                tword.RemoveAt(1);
            }
            if (tword[tword.Count() - 2].ToLower() == "s" && tword[tword.Count() - 1] == "0")
            {
                tword[tword.Count() - 2] = "s0";
                tword.RemoveAt(tword.Count() - 1);
            }
            if (tword[tword.Count() - 3].ToLower() == "s" && tword[tword.Count() - 1] == "0")
            {
                tword[tword.Count() - 3] = "s0";
                tword.RemoveAt(tword.Count() - 1);
                tword.RemoveAt(tword.Count() - 2);
            }
            tword.RemoveAll(x=>x==" ");
            for (int i = 1; i < 5; i++)
            {
                for (int j = 1; j < 5; j++)
                {
                    if (i > stateRows - 1 || j > stateCols - 1) states[i, j] = "";
                    states[i, j] = Regex.Replace(states[i, j], @"\s+", "");
                    states[i, j] = Regex.Replace(states[i, j], @"Q", "q");
                }
            }
        }

        private void Check_States()
        {
            if (!check)
            {
                check = true;
                for (int i = 1; i < stateRows; i++)
                {
                    for (int j = 1; j < stateCols; j++)
                    {
                        Check_State(box[i-1, j-1]);
                    }
                }
                Check_Word();
                Check_Number();
            }
        }

        private void Check_State(TextBox t)
        {
            string r = @"^q(0|1|2|3|4)\s{0,}?(0|1|2)\s{0,}?(c|r|l)$";
            Regex regex = new Regex(r, RegexOptions.IgnoreCase);

            if (t.Text != "" && !regex.IsMatch(t.Text))
            {
                t.Background = Brushes.Crimson;
                check = false;
            }
            else
            {
                t.Background = Brushes.MintCream;
            }
        }

        private void Check_Word()
        {
            string r = @"^S0?\s{0,}?((0|1|2)\s{0,}?){2,}S?\s{0,}?0?$";
            Regex regex = new Regex(r, RegexOptions.IgnoreCase);
            if (!regex.IsMatch(Word.Text))
            {
                check = false;
                Word.Background = Brushes.Crimson;
            }
            else
            {
                Word.Background = Brushes.MintCream;
                strword = Word.Text;
            }
        }

        private void Check_Number()
        {
            string r = @"^\d{1,6}$";
            Regex regex = new Regex(r);
            if (!regex.IsMatch(MaxSteps.Text))
            {
                check = false;
                MaxSteps.Background = Brushes.Crimson;
            }
            else
            {
                MaxSteps.Background = Brushes.MintCream;
                Int32.TryParse(MaxSteps.Text, out maxstep);
            }
        }

        private void MaxSteps_Text(object sender, RoutedEventArgs e)
        {
            Check_Number();
        }

        private void TextBox_Word(object sender, RoutedEventArgs e)
        {
            Check_Word();
        }

        private void TextBox1_Text(object sender, RoutedEventArgs e)
        {
            states[1, 1] = t1.Text;
            Check_State(t1);
        }

        private void TextBox2_Text(object sender, RoutedEventArgs e)
        {
            states[1, 2] = t2.Text;
            Check_State(t2);
        }

        private void TextBox3_Text(object sender, RoutedEventArgs e)
        {
            states[1, 3] = t3.Text;
            Check_State(t3);
        }

        private void TextBox4_Text(object sender, RoutedEventArgs e)
        {
            states[1, 4] = t4.Text;
            Check_State(t4);
        }

        private void TextBox5_Text(object sender, RoutedEventArgs e)
        {
            states[2, 1] = t5.Text;
            Check_State(t5);
        }

        private void TextBox6_Text(object sender, RoutedEventArgs e)
        {
            states[2, 2] = t6.Text;
            Check_State(t6);
        }

        private void TextBox7_Text(object sender, RoutedEventArgs e)
        {
            states[2, 3] = t7.Text;
            Check_State(t7);
        }

        private void TextBox8_Text(object sender, RoutedEventArgs e)
        {
            states[2, 4] = t8.Text;
            Check_State(t8);
        }

        private void TextBox9_Text(object sender, RoutedEventArgs e)
        {
            states[3, 1] = t9.Text;
            Check_State(t9);
        }

        private void TextBox10_Text(object sender, RoutedEventArgs e)
        {
            states[3, 2] = t10.Text;
            Check_State(t10);
        }

        private void TextBox11_Text(object sender, RoutedEventArgs e)
        {
            states[3, 3] = t11.Text;
            Check_State(t11);
        }

        private void TextBox12_Text(object sender, RoutedEventArgs e)
        {
            states[3, 4] = t12.Text;
            Check_State(t12);
        }

        private void TextBox13_Text(object sender, RoutedEventArgs e)
        {
            states[4, 1] = t13.Text;
            Check_State(t13);
        }

        private void TextBox14_Text(object sender, RoutedEventArgs e)
        {
            states[4, 2] = t14.Text;
            Check_State(t14);
        }

        private void TextBox15_Text(object sender, RoutedEventArgs e)
        {
            states[4, 3] = t15.Text;
            Check_State(t15);
        }

        private void TextBox16_Text(object sender, RoutedEventArgs e)
        {
            states[4, 4] = t16.Text;
            Check_State(t16);
        }
    }
}
