using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using System.IO;
using static SDK.Form1;
using System.Diagnostics;
using System.Collections;

namespace SDK
{

    public partial class Form1 : Form
    {

        public int n = 9;
        public Button[][] b;
        public int sz = 621 / 9;
        struct Current
        {
            public int x;
            public int y;
        }
        Current acurrent = new Current();
        //Boolean start = false;
        public Form1()
        {
            InitializeComponent();
        }

        #region[FormLoad]
        private void Form1_Load(object sender, EventArgs e)
        {
            createSudoku();
            acurrent.x = -1;
            acurrent.y = -1;
        }
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            label3.Text = e.KeyData.ToString();
            buttonInput_KeyDown(sender, e);

        }
        #endregion
        #region[createSDK]
        public void createSudoku()
        {
            b = new Button[n][];
            for (int i = 0; i < n; i++)
            {
                b[i] = new Button[n];
            }

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    b[i][j] = new Button();
                    b[i][j].Size = new Size(sz, sz);
                    if (i < 3 && j < 3)
                    {
                        b[i][j].BackColor = Color.FromName("Cornsilk");
                    }
                    else if (i >= 3 && i < 6 && j >= 3 && j < 6)
                    {
                        b[i][j].BackColor = Color.FromName("Cornsilk");
                    }
                    else if (i >= 6 && i < 9 && j >= 6 && j < 9)
                    {
                        b[i][j].BackColor = Color.FromName("Cornsilk");
                    }
                    else if (i >= 6 && i < 9 && j < 3)
                    {
                        b[i][j].BackColor = Color.FromName("Cornsilk");
                    }
                    else if (i >= 6 && i < 9 && j >= 6)
                    {
                        b[i][j].BackColor = Color.FromName("Cornsilk");
                    }
                    else if (i < 3 && j >= 6 && j < 9)
                    {
                        b[i][j].BackColor = Color.FromName("Cornsilk");
                    }
                    else
                    {
                        b[i][j].BackColor = Color.FromName("Khaki");
                    }
                    b[i][j].Text = " ";
                    b[i][j].ForeColor = Color.FromName("red");
                    b[i][j].Location = new Point(i * sz + 6, j * sz + 6);
                    b[i][j].Click += new EventHandler(button_Click);
                    groupBox1.Controls.Add(b[i][j]);
                }
            }
        }

        #endregion
        #region[buttonClick]
        private void buttonInput_Click(object sender, EventArgs e)
        {
            int x = acurrent.x;
            int y = acurrent.y;
            if (x >= 0 && y >= 0 && x <= 8 && y <= 8)
            {
                Button bt = (Button)sender;
                if (bt.Text != "0")
                {
                    b[x][y].Text = bt.Text;
                }
                else
                {
                    b[x][y].Text = " ";
                }
                groupBoxInput.Visible = false;
                acurrent.x = -1;
                acurrent.y = -1;
            }
        }
        private void buttonInput_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
            int x = acurrent.x;
            int y = acurrent.y;
            if (x >= 0 && y >= 0 && x <= 8 && y <= 8)
            {
                string text = e.KeyData.ToString();
                int result = checkKeyData(text);
                if (result >= 0 && result <= 9)
                {
                    if (result != 0)
                    {
                        b[x][y].Text = result.ToString();
                    }
                    else
                    {
                        b[x][y].Text = " ";
                    }
                    groupBoxInput.Visible = false;
                    acurrent.x = -1;
                    acurrent.y = -1;
                }
            }
        }

        private void button_Click(object sender, EventArgs e)
        {
            label1.Text = "";
            Button bt = (Button)sender;
            //bt.Text = (int.Parse(bt.Text) + 1).ToString();
            int x = int.Parse(bt.Location.X.ToString());
            x = x / (621 / 9);
            int y = int.Parse(bt.Location.Y.ToString());
            y = y / (621 / 9);
            label2.Text = (x + 1).ToString() + "," + (y + 1).ToString();
            acurrent.x = x;
            acurrent.y = y;
            groupBoxInput.Visible = true;

        }
        private void buttonImport_Click(object sender, EventArgs e)
        {
            label1.Text = "";
            groupBoxInput.Visible = false;
            acurrent.x = -1;
            acurrent.y = -1;
            Import();
        }
        private void buttonNew_Click(object sender, EventArgs e)
        {
            label1.Text = "";
            groupBoxInput.Visible = false;
            acurrent.x = -1;
            acurrent.y = -1;
            Clear();
        }
        private void buttonExport_Click(object sender, EventArgs e)
        {
            label1.Text = "";
            groupBoxInput.Visible = false;
            acurrent.x = -1;
            acurrent.y = -1;
            Export();
        }
        private void button10_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion
        #region[Solve]
        // Đặt biến useHeuristic ở mức lớp để có thể truy cập ở nhiều phương thức
        private bool useHeuristic;
        // Nếu có sự kiện chọn giải thuật (ví dụ: một nút radio) thì có thể cập nhật biến này
        private void HeuristicRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            useHeuristic = true;
        }
        private void BacktrackRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            useHeuristic = false;
        }
        private ISudokuSolver sudokuSolver;


        private void ChooseSolver(bool useHeuristic)
        {
            if (useHeuristic)
            {
                sudokuSolver = new HeuristicBacktrackSolver();
            }
            else
            {
                sudokuSolver = new BackTrackSolver();
            }
        }
        private void Solve_Click(object sender, EventArgs e)
        {
            if (checkPuzzle() == false)
            {
                MessageBox.Show("Vui lòng kiểm tra lại đề đã nhập!");
            }
            else
            {
                int[,] arr = new int[9, 9];
                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        if (b[i][j].Text != " ")
                        {
                            b[i][j].ForeColor = Color.FromName("Black");
                            arr[i, j] = int.Parse(b[i][j].Text);
                        }
                        else
                        {
                            arr[i, j] = 0;
                        }
                        b[i][j].Click -= new EventHandler(button_Click);
                    }
                }

                // Chọn giải thuật dựa trên biến useHeuristic
                ChooseSolver(useHeuristic);

                //* In kết quả
                if (sudokuSolver != null)
                {
                    if (sudokuSolver.Solve(arr)) // Kích hoạt hàm giải 
                    {
                        for (int i = 0; i < n; i++)
                        {
                            for (int j = 0; j < n; j++)
                            {
                                if (arr[i, j].ToString() != "0")
                                {
                                    b[i][j].Text = arr[i, j].ToString();
                                }
                                else
                                {
                                    b[i][j].Text = " ";
                                }
                            }
                        }
                    }
                    Solve.Enabled = false;
                }
                else
                {
                    MessageBox.Show("Không thể chọn giải thuật. Vui lòng kiểm tra lại.");
                }
                //*/
            }
            groupBoxInput.Visible = false;
            acurrent.x = -1;
            acurrent.y = -1;
        }

        #endregion
        #region[CheckAvailabe]
        private static bool IsAvailable(int[,] puzzle, int row, int col, int num)
        {
            int rowStart = (row / 3) * 3;
            int colStart = (col / 3) * 3;

            for (int i = 0; i < 9; ++i)
            {
                if (puzzle[row, i] == num) return false;
                if (puzzle[i, col] == num) return false;
                if (puzzle[rowStart + (i % 3), colStart + (i / 3)] == num) return false;
            }

            return true;
        }
        #endregion
        #region[CheckPuzzle]
        public bool checkPuzzle() //check đề nhập vào có trùng cột, hàng, trong ô hay không.
        {
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (b[i][j].Text != " ")
                    {
                        for (int k = i + 1; k < 9; ++k)  // kiem tra theo hang
                        {
                            if (b[k][j].Text != " ")
                            {
                                if (int.Parse(b[i][j].Text) == int.Parse(b[k][j].Text))
                                    return false;
                            }
                        }
                        for (int k = 0; k < i; ++k)  // kiem tra theo hang
                        {
                            if (b[k][j].Text != " ")
                            {
                                if (int.Parse(b[i][j].Text) == int.Parse(b[k][j].Text))
                                    return false;
                            }
                        }
                        for (int k = j + 1; k < 9; ++k)  // kiem tra theo hang
                        {
                            if (b[i][k].Text != " ")
                            {
                                if (int.Parse(b[i][j].Text) == int.Parse(b[i][k].Text))
                                    return false;
                            }
                        }
                        for (int k = 0; k < j; ++k)  // kiem tra theo hang
                        {
                            if (b[i][k].Text != " ")
                            {
                                if (int.Parse(b[i][j].Text) == int.Parse(b[i][k].Text))
                                    return false;
                            }
                        }
                        int boxRowOffset = (i / 3) * 3;
                        int boxColOffset = (j / 3) * 3;

                        for (int k = 0; k < 3; ++k) //kiem tra trong 9 ô nhỏ
                        {
                            for (int m = 0; m < 3; ++m)
                            {
                                if ((boxRowOffset + k) != i && boxColOffset + m != j)
                                {
                                    if (b[boxRowOffset + k][boxColOffset + m].Text != " ")
                                    {
                                        if (int.Parse(b[i][j].Text) == int.Parse(b[boxRowOffset + k][boxColOffset + m].Text))
                                            return false;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return true;
        }
        #endregion
        #region[Import]     
        private void Import()
        {
            OpenFileDialog op = new OpenFileDialog();
            op.Filter = "txt file|*.txt";
            if (op.ShowDialog() == DialogResult.OK)
            {
                Clear();
                string filename = op.FileName;
                label1.Text = "";
                string[] filelines = File.ReadAllLines(filename);
                if (filelines.Length == 9)
                {
                    for (int j = 0; j < filelines.Length; j++)
                    {
                        string[] splitLines = filelines[j].Split(' ');
                        if (splitLines.Length == 9)
                        {
                            for (int i = 0; i < splitLines.Length; i++)
                            {
                                if (int.Parse(splitLines[i]) >= 0 && int.Parse(splitLines[i]) <= 9)
                                {
                                    if (int.Parse(splitLines[i]) == 0)
                                    {
                                        b[i][j].Text = " ";
                                        b[i][j].ForeColor = Color.FromName("red");
                                    }
                                    else
                                    {
                                        b[i][j].Text = splitLines[i];
                                        b[i][j].ForeColor = Color.FromName("black");
                                        b[i][j].Click -= new EventHandler(button_Click);
                                    }
                                }
                                else
                                {
                                    label1.Text = "Number is out of range. Current number of spitlines[" +
                                        i.ToString() + "] = " + int.Parse(splitLines[i]).ToString();
                                    Clear();
                                    return;
                                }
                            }
                        }
                        else
                        {
                            label1.Text = "Your file hasn't contained enough data. SplitLines=" + splitLines.Length.ToString();
                            Clear();
                            return;
                        }
                    }
                }
                else
                {
                    label1.Text = "Error Reading File. Current file length is: " + filelines.Length.ToString();
                    Clear();
                    return;
                }
            }
            else
            {
                // File length less than 9
                MessageBox.Show("Your file doesn't have enough data. Please try with another file",
                                "Failed to load data", MessageBoxButtons.OK, MessageBoxIcon.Error);

                // Recall Import() function

            }
        }
        #endregion
        #region[Reset]
        private void Clear()
        {
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    b[i][j].Text = " ";
                    b[i][j].ForeColor = Color.FromName("red");
                    b[i][j].Click -= new EventHandler(button_Click);
                    b[i][j].Click += new EventHandler(button_Click);
                }
            }
            Solve.Enabled = true;
        }
        #endregion
        #region[Export]
        private void Export()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();

            saveFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            saveFileDialog.DefaultExt = "txt";
            saveFileDialog.FilterIndex = 2;
            saveFileDialog.RestoreDirectory = true;

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filename = saveFileDialog.FileName;
                string[] contents = new string[9];
                for (int j = 0; j < 9; j++)
                {
                    contents[j] = "";
                    for (int i = 0; i < 9; i++)
                    {
                        string anum = "";
                        if (b[i][j].Text == " ")
                        {
                            anum = "0";
                        }
                        else
                        {
                            anum = b[i][j].Text;
                        }
                        if (i < 8)
                        {
                            contents[j] += anum + " ";
                        }
                        else
                        {
                            contents[j] += anum;
                        }
                    }
                }
                File.WriteAllLines(filename, contents);
            }
        }
        #endregion
        #region[Numpad]
        private int checkKeyData(string data)
        {
            switch (data)
            {
                case "D1":
                case "NumPad1": return 1;
                case "D2":
                case "NumPad2": return 2;
                case "D3":
                case "NumPad3": return 3;
                case "D4":
                case "NumPad4": return 4;
                case "D5":
                case "NumPad5": return 5;
                case "D6":
                case "NumPad6": return 6;
                case "D7":
                case "NumPad7": return 7;
                case "D8":
                case "NumPad8": return 8;
                case "D9":
                case "NumPad9": return 9;
                case "D0":
                case "NumPad0":
                case "Back":
                case "Delete": return 0;
                default: return -1;
            }
        }
        #endregion

        public interface ISudokuSolver
        {
            bool Solve(int[,] arr);
        }
        
        public class HeuristicBacktrackSolver : ISudokuSolver
        {
            private int operationCount = 0; // đếm thao tác cơ bản
            private TimeSpan elapsedTime;

            public bool Solve(int[,] arr)
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                bool result = SolveSudoku(arr);

                stopwatch.Stop();
                elapsedTime = stopwatch.Elapsed;

                // Hiển thị thông báo sau khi giải xong
                ShowMessageBox(operationCount, elapsedTime);
                // reset bộ nhớ
                operationCount = 0;
                return result;
            }

            private static void ShowMessageBox(int operationCount, TimeSpan elapsedTime)
            {
                string message = $"Solved in {operationCount} operations.\nElapsed Time: {elapsedTime.TotalMilliseconds} milliseconds";
                MessageBox.Show(message, "Sudoku Solver Result (HeuristicBT)", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            private bool SolveSudoku(int[,] puzzle)
            {
                int row, col;

                // Tìm ô chưa được điền
                if (!FindUnassignedLocation(puzzle, out row, out col))
                {
                    // Nếu không còn ô chưa điền, đã giải xong
                    return true;
                }

                // Lấy danh sách các giá trị có thể thử theo hướng giảm độ ưu tiên
                List<int> possibleValues = GetPossibleValues(puzzle, row, col);

                // Thử các giá trị theo thứ tự giảm độ ưu tiên
                foreach (int num in possibleValues)
                {
                    puzzle[row, col] = num;

                    // Tăng giá trị bộ đếm thao tác cơ bản
                    operationCount++;

                    if (SolveSudoku(puzzle))
                    {
                        return true;
                    }

                    puzzle[row, col] = 0; // Quay lui nếu không tìm thấy giải pháp
                }

                return false;
            }

            private static List<int> GetPossibleValues(int[,] puzzle, int row, int col)
            {
                List<int> possibleValues = new List<int>();

                for (int num = 1; num <= 9; num++)
                {
                    if (IsSafe(puzzle, row, col, num))
                    {
                        possibleValues.Add(num);
                    }
                }

                // Sắp xếp giảm dần để ưu tiên các giá trị có ít khả năng xung đột
                possibleValues.Sort((a, b) =>
                {
                    int countA = CountConflicts(puzzle, row, col, a);
                    int countB = CountConflicts(puzzle, row, col, b);
                    return countA.CompareTo(countB);
                });

                return possibleValues;
            }

            private static int CountConflicts(int[,] puzzle, int row, int col, int num)
            {
                int count = 0;

                for (int i = 0; i < 9; i++)
                {
                    if (puzzle[row, i] == num || puzzle[i, col] == num || puzzle[row - row % 3 + i / 3, col - col % 3 + i % 3] == num)
                    {
                        count++;
                    }
                }

                return count;
            }

            private static bool FindUnassignedLocation(int[,] puzzle, out int row, out int col)
            {
                row = -1;
                col = -1;

                for (int i = 0; i < 9; i++)
                {
                    for (int j = 0; j < 9; j++)
                    {
                        if (puzzle[i, j] == 0)
                        {
                            row = i;
                            col = j;
                            return true;
                        }
                    }
                }

                return false;
            }

            private static bool IsSafe(int[,] puzzle, int row, int col, int num)
            {
                return !UsedInRow(puzzle, row, num) && !UsedInColumn(puzzle, col, num) && !UsedInBox(puzzle, row - row % 3, col - col % 3, num);
            }

            private static bool UsedInRow(int[,] puzzle, int row, int num)
            {
                for (int col = 0; col < 9; col++)
                {
                    if (puzzle[row, col] == num)
                    {
                        return true;
                    }
                }

                return false;
            }

            private static bool UsedInColumn(int[,] puzzle, int col, int num)
            {
                for (int row = 0; row < 9; row++)
                {
                    if (puzzle[row, col] == num)
                    {
                        return true;
                    }
                }

                return false;
            }

            private static bool UsedInBox(int[,] puzzle, int boxStartRow, int boxStartCol, int num)
            {
                for (int row = 0; row < 3; row++)
                {
                    for (int col = 0; col < 3; col++)
                    {
                        if (puzzle[row + boxStartRow, col + boxStartCol] == num)
                        {
                            return true;
                        }
                    }
                }

                return false;
            }
        }

        public class BackTrackSolver : ISudokuSolver
        {
            private static int operationCount = 0; // đếm thao tác cơ bản

            public bool Solve(int[,] arr)
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                bool result = SolveSudoku(arr);

                stopwatch.Stop();
                TimeSpan elapsedTime = stopwatch.Elapsed;

                // Hiển thị thông báo sau khi giải xong
                ShowMessageBox(operationCount, elapsedTime);
                // reset bộ đếm
                operationCount = 0;
                return result;
            }

            private static void ShowMessageBox(int operationCount, TimeSpan elapsedTime)
            {
                string message = $"Solved in {operationCount} operations.\nElapsed Time: {elapsedTime.TotalMilliseconds} milliseconds";
                MessageBox.Show(message, "Sudoku Solver Result (BackTrack)", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            private static bool SolveSudoku(int[,] puzzle)
            {
                int row, col;

                if (!FindUnassignedLocation(puzzle, out row, out col))
                {
                    return true;
                }

                for (int num = 1; num <= 9; num++)
                {
                    operationCount++;

                    if (IsSafe(puzzle, row, col, num))
                    {
                        puzzle[row, col] = num;

                        if (SolveSudoku(puzzle))
                        {
                            return true;
                        }

                        puzzle[row, col] = 0;
                    }
                }

                return false;
            }

            private static bool FindUnassignedLocation(int[,] puzzle, out int row, out int col)
            {
                for (row = 0; row < 9; row++)
                {
                    for (col = 0; col < 9; col++)
                    {
                        if (puzzle[row, col] == 0)
                        {
                            return true;
                        }
                    }
                }

                row = -1;
                col = -1;
                return false;
            }

            private static bool IsSafe(int[,] puzzle, int row, int col, int num)
            {
                return !UsedInRow(puzzle, row, num) &&
                       !UsedInColumn(puzzle, col, num) &&
                       !UsedInBox(puzzle, row - row % 3, col - col % 3, num);
            }

            private static bool UsedInRow(int[,] puzzle, int row, int num)
            {
                for (int col = 0; col < 9; col++)
                {
                    if (puzzle[row, col] == num)
                    {
                        return true;
                    }
                }
                return false;
            }

            private static bool UsedInColumn(int[,] puzzle, int col, int num)
            {
                for (int row = 0; row < 9; row++)
                {
                    if (puzzle[row, col] == num)
                    {
                        return true;
                    }
                }
                return false;
            }

            private static bool UsedInBox(int[,] puzzle, int boxStartRow, int boxStartCol, int num)
            {
                for (int row = 0; row < 3; row++)
                {
                    for (int col = 0; col < 3; col++)
                    {
                        if (puzzle[row + boxStartRow, col + boxStartCol] == num)
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
        }
    }
}

