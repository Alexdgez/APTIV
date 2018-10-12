using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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


namespace APTIV2
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        RegexLexer csLexer = new RegexLexer();
        bool load;
        List<string> palabrasReservadas;
        List<string> tipos;

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            using (StreamReader sr = new StreamReader(@"..\..\RegexLexer.cs"))
            {
                //tbxCode.Text = sr.ReadToEnd();

                csLexer.AddTokenRule(@"\s+", "ESPACIO", true);
                csLexer.AddTokenRule(@"\b[a-zA-Z][\w]*\b", "IDENTIFICADOR");
                csLexer.AddTokenRule("##[^\r\n]*", "COMENTARIO");
                csLexer.AddTokenRule(@"\d*\.?\d+", "NUMERO");
                csLexer.AddTokenRule(@"[;]", "FINLINEA");
                csLexer.AddTokenRule(@"[\(\)\{\}\[\]]", "AGRUPADOR");
                csLexer.AddTokenRule(@"[\--\++\^\+\-/*%]", "OPERARIT");
                csLexer.AddTokenRule(@"~~", "INCREMENTO");
                csLexer.AddTokenRule(@"``", "DECREMENTO");
                csLexer.AddTokenRule(@"=", "ASIGNADOR");
                csLexer.AddTokenRule(@"&&|>|<|==|>=|<=|!", "OPERLOG");

                palabrasReservadas = new List<string>() {"Momletme", "Dadletme", "YOLO", "Party", "Work",
                                                        "mainchick"};
                tipos = new List<string>() {"Bro", "friendoffriend", "friend", "partner",
                                                        "date", "sister"  };

                csLexer.Compile(RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.ExplicitCapture);

                load = true;
                AnalizeCode();
                tbxCode.Focus();
            }
        }

        private void AnalizeCode()
        {
            lvToken.Items.Clear();

            int n = 0, e = 0;

            foreach (var tk in csLexer.GetTokens(tbxCode.Text))
            {
                if (tk.Name == "ERROR") e++;

                if (tk.Name == "IDENTIFICADOR")
                    if (palabrasReservadas.Contains(tk.Lexema))
                        tk.Name = "RESERVADO";
                    else if (tipos.Contains(tk.Lexema))
                        tk.Name = "TIPOS";

                lvToken.Items.Add(tk);
                n++;
            }

            this.Title = string.Format("Analizador Lexico - {0} tokens {1} errores", n, e);
        }

        private void CodeChanged(object sender, TextChangedEventArgs e)
        {
            if (load)
                AnalizeCode();
        }

    }
}

