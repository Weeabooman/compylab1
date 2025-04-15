using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace compy
{
    public partial class Form1 : Form
    {

        private bool isTextChanged = false;
        private string currentFilePath = string.Empty; //переменная для хранения пути к файлу
        private readonly string keywordPattern = @"\b(function|return|if|else|for|while|break|continue|class|public|private|protected|echo|require)\b";
        private readonly string identifierPattern = @"\$\w+";
        private readonly string numberPattern = @"\b\d+(\.\d+)?\b";
        private readonly string operatorPattern = @"[+\-*/=<>!&|]+";
        private readonly string stringPattern = @"'([^']*)'|""([^""]*)""";
        private readonly string punctuationPattern = @"[\(\)\{\}\[\];,]";
        private readonly string invalidCharacterPattern = @"[^a-zA-Z0-9\s\$\(\)\{\}\[\];,+\-*/=<>!&|']";  // Все недопустимые символы

        public Form1()
        {
            InitializeComponent();
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Проверка, нужно ли сохранить изменения
            if (!PromptToSaveChanges())
            {
                // Если пользователь выбрал "Отмена", отменяем закрытие окна
                e.Cancel = true;
            }
        }

        private void создатьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!PromptToSaveChanges())
                return; // если пользователь нажал "Отмена" — ничего не делать

            richTextBox1.Clear();
            currentFilePath = string.Empty;
            isTextChanged = false;
        }

        private void открытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!PromptToSaveChanges()) return;

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                currentFilePath = openFileDialog.FileName;
                richTextBox1.Text = File.ReadAllText(currentFilePath);
                isTextChanged = false;
            }
        }

        private void сохранитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            isTextChanged = false;
            if (string.IsNullOrEmpty(currentFilePath))
            {
                сохранитьКакToolStripMenuItem_Click(sender, e);
            }
            else
            {
                File.WriteAllText(currentFilePath, richTextBox1.Text);
            }
        }

        private void сохранитьКакToolStripMenuItem_Click(object sender, EventArgs e)
        {
            isTextChanged = false;
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                currentFilePath = saveFileDialog.FileName;
                File.WriteAllText(currentFilePath, richTextBox1.Text);
            }
        }

        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (PromptToSaveChanges())
            {
                Application.Exit();
            }
        }

        private void отменитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.ActiveControl is RichTextBox activeBox && activeBox.CanUndo)
            {
                activeBox.Undo();
            }
        }
        private void повторитьToolStripMenuItem_Click(object sender, EventArgs e) => richTextBox1.Redo();
        private void вырезатьToolStripMenuItem_Click(object sender, EventArgs e) => richTextBox1.Cut();
        private void копироватьToolStripMenuItem_Click(object sender, EventArgs e) => richTextBox1.Copy();
        private void вставитьToolStripMenuItem_Click(object sender, EventArgs e) => richTextBox1.Paste();
        private void удалитьToolStripMenuItem_Click(object sender, EventArgs e) => richTextBox1.SelectedText = "";
        private void выделитьВсеToolStripMenuItem_Click(object sender, EventArgs e) => richTextBox1.SelectAll();

        private void вызовСправкиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
        "Описание функций меню\n\n" +

        "Меню «Файл»:\n" +
        "• Создать — очищает текущий текстовый документ.\n" +
        "• Открыть — открывает существующий .txt файл.\n" +
        "• Сохранить — сохраняет текущий документ.\n" +
        "• Сохранить как — сохраняет документ под новым именем.\n" +
        "• Выход — завершает работу программы.\n\n" +

        "Меню «Правка»:\n" +
        "• Отменить — отмена последнего действия.\n" +
        "• Повторить — повтор отменённого действия.\n" +
        "• Вырезать — вырезает выделенный текст.\n" +
        "• Копировать — копирует выделенный текст.\n" +
        "• Вставить — вставляет текст из буфера обмена.\n" +
        "• Удалить — удаляет выделенный текст.\n" +
        "• Выделить все — выделяет весь текст в редакторе.\n\n" +

        "Меню «Справка»:\n" +
        "• Вызов справки — показывает данное описание.\n" +
        "• О программе — сведения о приложении.",
        "Справка",
        MessageBoxButtons.OK,
        MessageBoxIcon.Information
                );
        }

        private void оПрограммеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Made by Nemchenko Egor", "О программе", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void richTextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            // Ctrl + Enter — добавляем новую строку в richTextBox1
            if (e.Control && e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                richTextBox1.AppendText(Environment.NewLine);  // добавляем новую строку
            }
            // При нажатии обычного Enter — переносим весь текст из richTextBox1 в richTextBox1_1
            else if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;

                // Переносим весь текст из richTextBox1 в richTextBox1_1 с сохранением всех строк
                richTextBox2.Text = richTextBox1.Text.Replace("\r\n", Environment.NewLine).Replace("\n", Environment.NewLine);
            }
        }

        private bool PromptToSaveChanges()
        {
            if (!isTextChanged) return true;

            var result = MessageBox.Show(
                "Сохранить изменения перед продолжением?",
                "Изменения не сохранены",
                MessageBoxButtons.YesNoCancel,
                MessageBoxIcon.Warning
            );

            if (result == DialogResult.Yes)
            {
                сохранитьToolStripMenuItem_Click(this, EventArgs.Empty);
                return true;
            }
            else if (result == DialogResult.No)
            {
                return true;
            }

            // Если Cancel
            return false;
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            isTextChanged = true;
        }

        private void пускToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AnalyzeText();
        }

        private void AppendResult(string text)
        {
            richTextBox2.AppendText(text + "\n");
        }
        private void AnalyzeText()
        {
            string text = richTextBox1.Text;

            // Очистка результатов анализа
            richTextBox2.Clear();

            // Поиск недопустимых символов
            MatchCollection invalidCharacters = Regex.Matches(text, invalidCharacterPattern);

            // Если есть недопустимые символы, выводим ошибку и не показываем результаты
            if (invalidCharacters.Count > 0)
            {
                // Создадим строку с ошибками
                string invalidChars = string.Join(", ", invalidCharacters.Cast<Match>().Select(m => m.Value));
                MessageBox.Show($"Обнаружены недопустимые символы: {invalidChars}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;  // Возвращаемся, не продолжая вывод лексем
            }

            // Поиск лексем
            MatchCollection keywords = Regex.Matches(text, keywordPattern);
            MatchCollection identifiers = Regex.Matches(text, identifierPattern);
            MatchCollection numbers = Regex.Matches(text, numberPattern);
            MatchCollection operators = Regex.Matches(text, operatorPattern);
            MatchCollection strings = Regex.Matches(text, stringPattern);
            MatchCollection punctuation = Regex.Matches(text, punctuationPattern);

            // Вывод лексем в resultsRichTextBox
            AppendResult("Ключевые слова:");
            foreach (Match match in keywords)
            {
                AppendResult(match.Value);
            }

            AppendResult("\nИдентификаторы:");
            foreach (Match match in identifiers)
            {
                AppendResult(match.Value);
            }

            AppendResult("\nЧисла:");
            foreach (Match match in numbers)
            {
                AppendResult(match.Value);
            }

            AppendResult("\nОператоры:");
            foreach (Match match in operators)
            {
                AppendResult(match.Value);
            }

            AppendResult("\nСтроки:");
            foreach (Match match in strings)
            {
                AppendResult(match.Value);
            }

            AppendResult("\nПунктуация:");
            foreach (Match match in punctuation)
            {
                AppendResult(match.Value);
            }
        }
    }
}

