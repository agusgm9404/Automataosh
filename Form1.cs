using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.IO;

namespace autapp
{
	public partial class MainF : Form
	{

		public MainF()
		{
			InitializeComponent();

			Builder();
			TxtRgx.Text = File.ReadAllText(@"..\\..\\Docs\\Regex.txt");
		}

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.F5)
            {
                BtnVerif.PerformClick();
                return true;
            }
            else if (keyData == Keys.F2)
            {
                PanelCtrl();
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        public void Espacio(char[] Arre, ref char[] Aux)
		{
			int cN = 0;
			int j = 0, k = 0;
			bool uno = false;
			for (int i = 0; i < Arre.Length; i++)
			{
				if (coincide(oper, Arre[i]) || coincide(delim, Arre[i]) || coincide(rela, Arre[i]))//coincide regla debe hacerse con otro metodo
				{
					if (Arre[i - 1] != ' ')
						cN++;
					if (i + 1 < Arre.Length)
					{
						if (Arre[i + 1] != ' ')//VALIDAR QUE NO SEA MAYOR QUE I
							cN++;
					}
				}
			}

			//cN++;
			if (cN == 0)
			{

			}
			if (cN != 0)
			{
				Aux = new char[Arre.Length + cN + 1];

				while (k < Arre.Length)
				{
					if (j == 240)//86,136,137
					{

					}
					if (coincide(oper, Arre[k]) || coincide(delim, Arre[k]) || coincide(rela, Arre[k]))
					{
						if (Arre[k - 1] != ' ')
						{
							Aux[j] = ' ';
							j++;
							Aux[j] = Arre[k];
							j++;
							uno = true;
						}
						if (Arre[k] != ' ' && uno == false)//
						{
							Aux[j] = Arre[k];
							j++;
						}
						if (k + 1 < Arre.Length)
						{
							if (Arre[k + 1] != ' ')
							{
								Aux[j] = ' ';
								j++;
							}
						}
					}
					else
					{
						Aux[j] = Arre[k];
						j++;
					}
					k++;
					uno = false;
				}

				Aux[Aux.Length - 1] = ' ';
			}
			else
			{
				Aux = new char[Arre.Length + 1];

				for (int i = 0; i < Arre.Length; i++)
				{
					Aux[i] = Arre[i];
				}
				Aux[Aux.Length - 1] = ' ';
			}
		}

		private void BtnVerif_Click(object sender, EventArgs e)
		{
            cont = 0;
            if (!string.IsNullOrWhiteSpace(RichTxt.Text))
            {
                if (cont == 0 && RichTxt.Text.Length == 6)
                {
                    if (RichTxt.Text.Substring(0, 6) == "SELECT")
                    {
                        prse = true;
                        cont++;
                    }
                    else if ((new string[] { "CREATE", "INSERT" }).Contains<string>(RichTxt.Text.Substring(0, 6)))
                    {
                        prse = false;
                        cont++;
                    }
                    else
                    {
                        setStatus("Error 2:205 Línea 1 Falta palabra reservada.");
                        if (BtnOpenResults.Visible)
                        {
                            PanelCtrl();
                        }
                    }
                }
                else
                {
                    setStatus("Error 2:205 Línea 1 Falta palabra reservada.");
                    if (BtnOpenResults.Visible)
                    {
                        PanelCtrl();
                    }
                }
                if (cont!=0)
                {
                    if (Action())
                    {
                        if (BtnOpenResults.Visible)
                        {
                            PanelCtrl();
                        }
                    }
                    else
                    {
                        if (!BtnOpenResults.Visible)
                        {
                            PanelCtrl();
                        }
                    }
                }
            }
            else
            {
                LblStatus.Text = " ";
                PnlBtnResults.Show();
                PnlResults.Hide();
                DgvData.Rows.Clear();
            }
		}

        private bool Action()
        {
            try
            {
                iden = new string[20, 3];
                Vacio(ref iden, "i");
                cons = new string[70, 3];
                Vacio(ref cons, "cons");

                ccons = 0;
                vcons = 63;
                ciden = 0;
                viden = 101;
                x = 0;
                est = 0;
                pal = "";
                car = ' ';
                cdgv = 0;
                colum = 0;
                cNum = 0;
                cLin = 1;
                error = false;
                alfa = false;
                idco = false;
                codidco = 0;
                calfa = 0;
                

                DgvData.Rows.Clear();
                constValue = 63;
                identValue = 101;
                iden = new string[20, 3];
                Vacio(ref iden, "i");
                cons = new string[70, 3];
                Vacio(ref cons, "cons");
                cad = RichTxt.Text.Trim().ToUpper();
                Aest = cad.ToCharArray();
                pila = new Stack<string>();
                lislex = new List<string>();
                lisError = new List<string>();
                lisErCLin = new List<string>();

                Espacio(Aest, ref Aux);

                Aest = Aux;
                for (int i = 0; i < Aest.Length; i++)
                {
                    car = Aest[i];
                    ValidarEst(car, Estados, ref est, DgvData, lislex, lisErCLin);
                }
                if (error)
                {
                    for (int i = 0; i < lisError.Count; i++)
                    {
                        setStatus(lisError[i]);
                        return false;
                    }
                }
                lislex.Add("$");
                RichTxt.Focus();
                if (prse)
                    return parse();
                else
                    return ddl();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

		private void RichTxt_KeyUp(object sender, KeyEventArgs e)
		{
			if (IsValid(RichTxt.Text))
			{
				//LblStatus.Text = "Válido";
			}
			else
			{
				//LblStatus.Text = "No válido";
			}

			if (e.KeyValue == 219)
			{
				if (RichTxt.SelectionColor == Color.LightSalmon)
				{
					RichTxt.SelectionColor = Color.WhiteSmoke;
				}
				else
				{
					wordColor(Color.LightSalmon, RichTxt.TextLength - 1, RichTxt.TextLength);
					RichTxt.SelectionColor = Color.LightSalmon;
				}
			}
		}

		string[,] matriz2 ={
						//        s0 10    *1 11     ,2 12   .3 13   i4 14   f5 15   r6 50   y7 51   o8 52   w9 53   '10 54  a11 60  d12 61  $13 62  n14 72  )15 100    
					   /*Q0 300*/{"sAfFJ"  ,null     ,null   ,null   ,null   ,null   ,null   ,null   ,null   ,null   ,null   ,null   ,null   ,null   ,null   ,null},
						/*A1 301*/{null    , "*"     ,null   ,null   , "B"   ,null   ,null   ,null   ,null   ,null   ,null   ,null   ,null   ,null   ,null   ,null}, 
						/*B2 302*/{null    ,null     ,null   ,null   ,"CD"   ,null   ,null   ,null   ,null   ,null   ,null   ,null   ,null   ,null   ,null   ,null},
						/*D3 303*/{null    ,null     ,",B"   ,null   ,null   , "0"   ,null   ,null   ,null   ,null   ,null   ,null   ,null   ,null   ,null   ,null},
						/*C4 304*/{null    ,null     ,null   ,null   ,"iE"   ,null   ,null   ,null   ,null   ,null   ,null   ,null   ,null   ,null   ,null   ,null},
						/*E5 305*/{null    ,null     , "0"   ,".i"   ,null   ,"0"    , "0"   , "0"   , "0"   ,null   ,null   ,null   ,null   ,null    ,"0"    ,"0"},//$"0"
						/*F6 306*/{null    ,null     ,null   ,null   ,"GH"   ,null   ,null   ,null   ,null   ,null   ,null   ,null   ,null   ,null   ,null   ,null},
						/*H7 307*/{null    ,null     ,",F"   ,null   ,null   ,null   ,null   ,null   ,null   , "0"   ,null   ,null   ,null   , "0"   ,null   ,"0"},
						/*G8 308*/{null    ,null     ,null   ,null   ,"iI"   ,null   ,null   ,null   ,null   ,null   ,null   ,null   ,null   ,null   ,null   ,null},
						/*I9 309*/{null    ,null     ,"0"    ,null   , "i"   ,null   ,null   ,null   ,null   , "0"   ,null   ,null   ,null   , "0"   ,null   ,"0"},
						/*J10 310*/{null   ,null    ,null   ,null   ,null   ,null   ,null   ,null   ,null   ,"wK"   ,null   ,null   ,null   , "0"   ,null   ,"0"},
						/*K11 311*/{null   ,null    ,null   ,null   ,"LV"   ,null   ,null   ,null   ,null   ,null   ,null   ,null   ,null   ,null   ,null   ,null},
						/*V12 312*/{null   ,null    ,null   ,null   ,null   ,null   ,null   ,"PK"   ,"PK"   ,null   ,null   ,null   ,null   , "0"   ,null   ,"0"},
						/*L13 313*/{null   ,null    ,null   ,null  ,"CM"    ,null   ,null   ,null   ,null   ,null   ,null   ,null   ,null   ,null   ,null   ,null},//i=COV
						/*M14 314*/{null   ,null    ,null   ,null  , null   ,null   ,"NO"   ,null   ,null   ,null   ,null   ,null   ,null   ,null   ,"n(Q)" ,null},
						/*N15 315*/{null   ,null,   null    ,null  ,null    ,null   ,"r"    , null  , null  ,null   ,null   ,null   ,null   ,null   ,null   ,null},
						/*O16 316*/{null   ,null    ,null   ,null  ,"C"     ,null   ,null   ,null   ,null   ,null   ,"'R'"  ,null   ,"T"    ,null   ,null   ,null},
						/*P17 317*/{null   ,null    ,null   ,null  ,null    ,null   ,null   ,"y"    ,"o"    ,null   ,null   ,null   ,null   ,null   ,null   ,null},
						/*R18 318*/{null   ,null    ,null   ,null  ,null    ,null   ,null   ,null   ,null   ,null   ,null   ,"a"    ,null   ,null   ,null   ,null},
						/*T19 319*/{null   ,null    ,null   ,null  ,null    ,null   ,null   ,null   ,null   ,null   ,null   ,null   ,"d"    ,null   , null  ,null},};

		string[,] DDL ={       /*c*/    /*m*/       /*h*/  /*u*/   /*e*/   /*,*/   /*i*/       /*b*/       /*p*/   /*j*/   /*l*/        /*'*/   /*d*/   /*)*/   /*$*/
					/*C*/{"cti(A)B"    ,null       ,null   ,null   ,null   ,null   ,null       ,null       ,null   ,null,  null        ,null   ,null   ,null   ,null},
					/*B*/{  "C"         ,"I"        ,null   ,null   ,null   ,null   ,null       ,null       ,null   ,null   ,null       ,null   ,null   ,null   ,"0"},
					/*A*/{  null        ,null       ,null   ,null   ,null   ,null   ,"iD(d)EF"  ,null       ,null   ,null   ,null       ,null   ,null   ,null   ,null },
					/*D*/{null          ,null       ,"h"    ,"u"    ,null   ,null   ,null       ,null       ,null   ,null   ,null       ,null   ,null   ,null   ,null},
					/*E*/{null          ,null       ,null   ,null   ,"eg"   ,"0"    ,null       ,null       ,null   ,null   ,null       ,null   ,null   ,"0"   ,null},
					/*F*/{null          ,null       ,null   ,null   ,null   ,",G"   ,null       ,"H"       ,null   ,null   ,null       ,null   ,null   ,"0"    ,null},//b["H"]**
					/*G*/{null          ,null       ,null   ,null   ,null   ,null   ,"A"        ,"H"        ,null   ,null   ,null       ,null   ,null   ,null   ,null},
					/*H*/{null          ,null       ,null   ,null   ,null   ,null   ,null       ,"biJ(i)K"  ,null   ,null   ,null       ,null   ,null   ,null   ,null},
					/*J*/{null          ,null       ,null   ,null   ,null   ,null   ,null       ,null       ,"pk"   ,"jk"   ,null       ,null   ,null   ,null   ,null},
					/*K*/{null          ,null       ,null   ,null   ,null   ,",H"   ,null       ,null       ,null   ,null   ,"li(i)L"   ,null   ,null   ,"0"    ,null},
					/*L*/{null          ,null       ,null   ,null   ,null   ,",H"   ,null       ,null       ,null   ,null   ,null       ,null   ,null   ,"0"    ,null},
					/*I*/{null          ,"mqiv(M);N",null   ,null   ,null   ,null   ,null       ,null       ,null   ,null   ,null       ,null   ,null   ,null   ,null},
					/*M*/{null          ,null       ,null   ,null   ,null   ,null   ,null       ,null       ,null   ,null   ,null       ,"OP"   ,"OP"   ,null   ,null},
					/*O*/{null          ,null       ,null   ,null   ,null   ,null   ,null       ,null       ,null   ,null   ,null       ,"'a'"  ,"d"    ,null   ,null},
					/*P*/{null          ,null       ,null   ,null   ,null   ,",M"   ,null       ,null       ,null   ,null   ,null       ,null   ,null   ,"0"    ,null},
					/*N*/{"C"           ,"I"        ,null   ,null   ,null   ,null   ,null       ,null       ,null   ,null   ,null       ,null   ,null   ,null   ,"0"}};
		/*List<Tuple<int, int, string>> matrixList = new List<Tuple<int, int, string>>()
  {
			new Tuple<int, int, string>(300, 10, "10 301 11 306 310"),
			new Tuple<int, int, string>(301, 100 | 72, "302 72"),
			new Tuple<int, int, string>(302, 100, "304 303"),
			new Tuple<int, int, string>(303, 50 | 99 , "50 302"),
			new Tuple<int, int, string>(304, 100, "100 305"),
			new Tuple<int, int, string>(305, 51 | 99, "51 100 | 99"),
			new Tuple<int, int, string>(306, 100, "308 307"),
			new Tuple<int, int, string>(307, 50 | 99, "50 306 | 99"),
			new Tuple<int, int, string>(308, 100, "100 309"),
			new Tuple<int, int, string>(309, 100 | 99 , "100 99"),
			new Tuple<int, int, string>(310, 12 | 99, "12 311 | 99"),
			new Tuple<int, int, string>(311, 100, "313 312"),
			new Tuple<int, int, string>(312, 14 | 15, "317 311 | 99"),
			new Tuple<int, int, string>(313, 100, "304 314"),
			new Tuple<int, int, string>(314, 60| 13, "315 316 | 13 52 300 53"),
			new Tuple<int, int, string>(315, 60, "60"),
			new Tuple<int, int, string>(316, 100 | 54 | 61, "304 | 54 318 54 | 319"),
			new Tuple<int, int, string>(317, 14 | 15, "14 | 15"),
			new Tuple<int, int, string>(318, 62, "62"),
			new Tuple<int, int, string>(319, 61, "61")
		};*/

		bool EsTerminalDML(string x)
		{
			//s      *    ,    .    i    f    r    y   o    w    '    a     d    $    n     p     
			if (x == "s")
				return true;
			else if (x == "*")
				return true;
			else if (x == ",")
				return true;
			else if (x == ".")
				return true;
			else if (x == "i")
				return true;
			else if (x == "f")
				return true;
			else if (x == "r")
				return true;
			else if (x == "y")
				return true;
			else if (x == "o")
				return true;
			else if (x == "w")
				return true;
			else if (x == "'")
				return true;
			else if (x == "a")
				return true;
			else if (x == "d")
				return true;
			else if (x == "$")
				return true;
			else if (x == "n")
				return true;
			else if (x == ")" || x == "(")
				return true;

			return false;


		}

		bool coincidepalErr(string[,] Array, string pal)
		{
			for (int i = 0; i < (Array.Length / 3); i++)
			{
				if (Array[i, 2].Contains(pal))
					return true;
			}
			return false;
		}

		void ErroresXK(string x, string k, int apuntador, List<string> lisErCLin)
		{
			int apu = 0;

			if (apuntador < lisErCLin.Count)
			{
				//if (apuntador - 1 > 0)
				//    apu = apuntador - 1;
				//else
				apu = apuntador;
			}
			else
				apu = lisErCLin.Count - 1;

			if ((x == "(" || x == ")" || x == "$") && (k == "s" || k == "c" || k == "n" || k == ")"))
			{
				setStatus("Error 2:202 Línea " + lisErCLin[apu] + " Falta el delimitador “(”.");
				//MessageBox.Show("Error 2:202 Línea " + lisErCLin[apu] + " Falta el delimitador “(”.", "Parse");//()
				return;
			}
			else if ((x == "(" || x == ")" || x == "$") && k == "$" || k == "(")
			{
				setStatus("Error 2:202 Línea " + lisErCLin[apu] + " Falta el delimitador “)”.");
				//MessageBox.Show("Error 2:202 Línea " + lisErCLin[apu] + " Falta el delimitador “)”.", "Parse");//()
				return;
			}
			else 
            {
                //setStatus("Error en x==k .");
                //MessageBox.Show("Error en x==k .", "Parse");//()
                LblStatus.Text += '.';
				return;
			}
		}

		void valorDML(string x, string k, ref int col, ref int fila)
		{
			//Q A1 B2 D3 C4 E5 F6 H7 G8 I9 J10 K11 V12 L13 M14 N15 O16 P17 R18 T19
			//colum=Fx
			if (x == "Q")
				col = 0;
			else if (x == "A")
				col = 1;
			else if (x == "B")
				col = 2;
			else if (x == "D")
				col = 3;
			else if (x == "C")
				col = 4;
			else if (x == "E")
				col = 5;
			else if (x == "F")
				col = 6;
			else if (x == "H")
				col = 7;
			else if (x == "G")
				col = 8;
			else if (x == "I")
				col = 9;
			else if (x == "J")
				col = 10;
			else if (x == "K")
				col = 11;
			else if (x == "V")
				col = 12;
			else if (x == "L")
				col = 13;
			else if (x == "M")
				col = 14;
			else if (x == "N")
				col = 15;
			else if (x == "O")
				col = 16;
			else if (x == "P")
				col = 17;
			else if (x == "R")
				col = 18;
			else if (x == "T")
				col = 19;
			//Q A B D C E G I H J K L U M V N O P S T

			//      s0     *1   ,2   .3   i4   f5   r6   y7   o8   w9   '10  a11  d12  $13  n14  p15
			if (k == "s")
				fila = 0;
			else if (k == "*")
				fila = 1;
			else if (k == ",")
				fila = 2;
			else if (k == ".")
				fila = 3;
			else if (k == "i")
				fila = 4;
			else if (k == "f")
				fila = 5;
			else if (k == "r")
				fila = 6;
			else if (k == "y")
				fila = 7;
			else if (k == "o")
				fila = 8;
			else if (k == "w")
				fila = 9;
			else if (k == "'")
				fila = 10;
			else if (k == "a")
				fila = 11;
			else if (k == "d")
				fila = 12;
			else if (k == "$")
				fila = 13;
			else if (k == "n")
				fila = 14;
			else if (k == ")" || k == "(")
				fila = 15;

			//s      *    ,    .    i    f    l    y   o    w    '    a     d    $    n     p     

		}
		void valorDDL(string x, string k, ref int col, ref int fila, int apuntador)
		{
			//C0 B1 A2 D3 E4 F5 G6 H7 J8 K9 L10 I11 M12 O13 P14 N15
			//colum=x
			if (x == "C")
				col = 0;
			else if (x == "B")
				col = 1;
			else if (x == "A")
				col = 2;
			else if (x == "D")
				col = 3;
			else if (x == "E")
				col = 4;
			else if (x == "F")
				col = 5;
			else if (x == "G")
				col = 6;
			else if (x == "H")
				col = 7;
			else if (x == "J")
				col = 8;
			else if (x == "K")
				col = 9;
			else if (x == "L")
				col = 10;
			else if (x == "I")
				col = 11;
			else if (x == "M")
				col = 12;
			else if (x == "O")
				col = 13;
			else if (x == "P")
				col = 14;
			else if (x == "N")
				col = 15;
			//x=="g"

			/*c0   m1*/ /*h2*/ /*u3*/ /*e4*//*,5*//*i6*//*b7*//*p8*//*j9*//*l10*//*'11*//*d12*/ /*)13*//*$14*/
			if (k == "c")
				fila = 0;
			else if (k == "m")
				fila = 1;
			else if (k == "h")
				fila = 2;
			else if (k == "u")
				fila = 3;
			else if (k == "e")
				fila = 4;
			else if (k == ",")
				fila = 5;
			else if (k == "i")
				fila = 6;
			else if (k == "b")
				fila = 7;
			else if (k == "p")
				fila = 8;
			else if (k == "j")
				fila = 9;
			else if (k == "l")
				fila = 10;
			else if (k == "'")
				fila = 11;
			else if (k == "d")
				fila = 12;
			else if (k == ")" || k == "(")
				fila = 13;
			else if (k == "$")
				fila = 14;
			else if (apuntador < 16)
				fila = apuntador;

		}

		void Inversamente(string[,] Matriz2, int col, int fila, Stack<string> pila)
		{
			string aux = "";
			aux = Matriz2[col, fila];
			//aux = Matriz2[fila, col];
			char car = ' ';
			for (int i = aux.Length - 1; i >= 0; i--)
			{
				car = aux[i];
				pila.Push(Convert.ToString(car));
			}
		}

		void ErroresMatriz(string x, string k, string kant, int apuntador, List<string> lisErCLin, string palResANt)
		{
			int apu = 0;
			//x=M,k="'",kant=i

			if (apuntador < lisErCLin.Count)
			{
				//if (apuntador - 1 > 0)
				//    apu = apuntador - 1;
				//else
				apu = apuntador;
			}
			else
				apu = lisErCLin.Count - 1;

			if (/*(kant == "i" && k == "'" && x == "E") ||*/ (k == "$" && kant == "i" && x == "E") || (kant == "r" && k == "$" && x == "O"))
			{
				setStatus("Error 2:202 Línea " + lisErCLin[apu] + " Falta el delimitador apostrofo.");
				//MessageBox.Show("Error 2:202 Línea " + lisErCLin[apu] + " Falta el delimitador apostrofo.", "Parse");
				return;
			}
			else if (kant == "," && (coincidepalErr(palRes, k)))
			{
				setStatus("Error 2:203 Línea " + lisErCLin[apu] + " Falta atributo.");
				//MessageBox.Show("Error 2:203 Línea " + lisErCLin[apu] + " Falta atributo.", "Parse");
				return;
			}
			else if (coincidepalErr(palRes, k) && coincidepalErr(palRes, kant))
			{
				setStatus("Error 2:203 Línea " + lisErCLin[apu] + " Falta atributo.");
				//MessageBox.Show("Error 2:203 Línea " + lisErCLin[apu] + " Falta atributo.", "Parse");
				return;
			}
			else if (/*(kant == "'" && k == "i" && x == "V") ||*/ (kant == "d" && k == "i" && x == "V") || (kant == "i" && k == "i" && x == "V") || (kant == "i" && k == "." && x == "M"))
			{
				setStatus("Error 2:204 Línea " + lisErCLin[apu] + " Falta Operador.");
				//MessageBox.Show("Error 2:204 Línea " + lisErCLin[apu] + " Falta Operador.", "Parse");
				return;
			}
			else if ((kant == "i" && (k == "(" || k == ")") && x == "M") || (kant == "i" && k == "i" && x == "E") || ((kant == "i" && k == "n") || (kant == "i" && k == "r") && x == "H") || ((kant == "" && k == "i") || ((kant == "(" || kant == ")") && k == "i") && x == "Q"))
			{
				setStatus("Error 2:205 Línea " + lisErCLin[apu] + " Falta palabra reservada.");
				//MessageBox.Show("Error 2:205 Línea " + lisErCLin[apu] + " Falta palabra reservada.", "Parse");
				return;
			}
			else if (kant == "i" && (k == "'" || k == "d" || k == "i") && x == "M" && palResANt == "w")
			{
				setStatus("Error 2:204 Línea " + lisErCLin[apu] + " Falta Operador Relacional.");
				//MessageBox.Show("Error 2:204 Línea " + lisErCLin[apu] + " Falta Operador Relacional.", "Parse");
				return;
			}
			else if (kant == "w" && x == "K")
			{
				setStatus("Error 2:205 Línea " + lisErCLin[apu] + " Falta palabra reservada.");
				return;
			}
			else if (kant == "'" && (k == "r" || k == "d") && x == "V")
			{
				setStatus("Error 2:205 Línea " + lisErCLin[apu] + " Falta palabra reservada.");
				return;
			}
			else if (kant == "'" && k == "i" && x == "V")
			{
				setStatus("Error 2:205 Línea " + lisErCLin[apu] + " Falta apostrofo.");
				return;
			}
			else if (kant == "r" && k == "y" && x == "O")
			{
				setStatus("Error 2:203 Línea " + lisErCLin[apu] + " Falta atributo.");
				return;
				//MessageBox.Show("Error x == k.", "Parse");
			}
			else if (kant == "i" && k == "w" && x == "E")
			{
				setStatus("Error 2:205 Línea " + lisErCLin[apu] + " Falta palabra reservada.");
				return;
			}
			else if (kant == "f" && k == "," && x == "F")
			{
				setStatus("Error 2:203 Línea " + lisErCLin[apu] + " Falta atributo.");
				return;
			}
			else if (kant == "i" && k == "'" && x == "E")
			{
				setStatus("Error 2:204 Línea " + lisErCLin[apu] + " Falta Operador.");
				return;
			}
			else if (kant == "i" && k == "y" && x == "M")
			{
				setStatus("Error 2:204 Línea " + lisErCLin[apu] + " Falta Operador Relacional.");
				return;
			}
			else if (kant == "y" && k == "'" && x == "K")
			{
				setStatus("Error 2:202 Línea " + lisErCLin[apu] + " Falta el delimitador apostrofo.");
				return;
			}
			else if (kant == "y" && k == "r" && x == "K")
			{
				setStatus("Error 2:203 Línea " + lisErCLin[apu] + " Falta atributo.");
				return;
			}
			else if (kant == "'" && k == "'" && x == "V")
			{
				setStatus("Error 2:205 Línea " + lisErCLin[apu] + " Falta palabra reservada.");
				return;
			}
			else
			{
                LblStatus.Text += ".";
				return;
			}

			//else if()
			//{

			//}
		}

		bool coincide(string[,] Aray, char car)
		{

			for (int i = 0; i < (Aray.Length / 3); i++)
			{
				if (Aray[i, 0].Contains(Convert.ToString(car)))
					return true;
			}
			return false;
		}

		int QtipPal(string pal, string[,] Arreglo, ref int tipo)//palabras Reservadas
		{
			for (int i = 0; i < (Arreglo.Length / 3); i++)
			{
				if (Arreglo[i, 0].Contains(pal))
				{
					tipo = 1;
					return tipo;
				}
			}
			// tipo = 4;//si no esta en palabras reservadas es un identificador
			return tipo;
		}

		bool coincidepal(string[,] Aray, string pal)
		{
			for (int i = 0; i < (Aray.Length / 3); i++)
			{
				if (Aray[i, 0].Contains(pal))
					return true;
			}
			return false;
		}

		void LlenarArreglo(string pal, ref int vArreglo, ref int cArreglo, ref string[,] Arreglo, ref int tipo, ref int codidco)
		{
			if (cArreglo < (Arreglo.Length / 2))
			{
				idco = true;
				Arreglo[cArreglo, 0] = pal;
				Arreglo[cArreglo, 1] = vArreglo.ToString();
				codidco = vArreglo;
				if (Arreglo[0, 1] == "101")
					tipo = 100;//Identificador
				else if (Arreglo[0, 1] == "63")
					tipo = 62;//constante
				cArreglo++;
				vArreglo++;
			}
		}

		bool coinciapost(string pal)
		{
			for (int i = 0; i < (delim.Length / 3) - 1; i++)
			{
				if (delim[i, 0].Contains(pal))
					return true;
			}

			return false;
		}

		void Vacio(ref string[,] Arreglo, string x)
		{
			for (int i = 0; i < (Arreglo.Length / 3); i++)
			{
				for (int j = 0; j < 3; j++)
				{
					if (j < 2)
						Arreglo[i, j] = "";
					else
						Arreglo[i, j] = x;

				}
			}
		}



		string codificacion(string pal, ref int tipo)
		{
			string codificacion = "";
			for (int i = 0; i < (palRes.Length / 3); i++)
			{
				if (palRes[i, 0].Contains(pal))
				{
					codificacion = palRes[i, 1];

					return codificacion;
				}
			}

			for (int i = 0; i < (delim.Length / 3); i++)
			{
				if (delim[i, 0].Contains(pal))
				{
					codificacion = delim[i, 1];

					return codificacion;
				}
			}
			for (int i = 0; i < (oper.Length) / 3; i++)
			{
				if (oper[i, 0].Contains(pal))
				{
					codificacion = oper[i, 1];

					return codificacion;
				}
			}
			for (int i = 0; i < (rela.Length) / 3; i++)
			{
				if (rela[i, 0].Contains(pal))
				{
					codificacion = rela[i, 1];

					return codificacion;
				}
			}
			for (int i = 0; i < (iden.Length) / 3; i++)
			{
				if (iden[i, 0].Contains(pal))
				{
					codificacion = iden[i, 1];

					return codificacion;
				}
			}
			for (int i = 0; i < (cons.Length) / 3; i++)
			{
				if (cons[i, 0].Contains(pal))
				{
					codificacion = cons[i, 1];

					return codificacion;
				}
			}

			return null;
		}

		void AgregarTokens(List<string> lislex, string pal)
		{
			string token = "";
			if (pal == "SELECT")//10
				token = "s";
			else if (pal == "FROM")//11
				token = "f";
			else if (pal == "WHERE")//12
				token = "w";
			else if (pal == "IN")//13
				token = "n";
			else if (pal == "AND")//14
				token = "y";
			else if (pal == "OR")//15
				token = "o";
			else if (pal == "CREATE")//16
				token = "c";
			else if (pal == "TABLE")//17
				token = "t";
			else if (pal == "CHAR")//18
				token = "h";
			else if (pal == "NUMERIC")//19
				token = "u";
			else if (pal == "NOT")//20
				token = "e";
			else if (pal == "NULL")//21
				token = "g";
			else if (pal == "CONSTRAINT")//22
				token = "b";
			else if (pal == "KEY")//23
				token = "k";
			else if (pal == "PRIMARY")//24
				token = "p";
			else if (pal == "FOREIGN")//25
				token = "j";
			else if (pal == "REFERENCES")//26
				token = "l";
			else if (pal == "INSERT")//27
				token = "m";
			else if (pal == "INTO")//28
				token = "q";
			else if (pal == "VALUES")//29
				token = "v";
			else if (pal == "DISTINCT")//30
				token = "d";//Palabras Reservadas  de c a d**
			else if (pal == ",")//50
				token = ",";
			else if (pal == ".")//51
				token = ".";
			else if (pal == "(")//52
				token = "(";
			else if (pal == ")")//53
				token = ")";
			else if (pal == "'")//54**AGREGADO
				token = "'";
			else if (pal == ";")//55**AGREGADO
				token = ";";
			else if (pal == "+")//70  AGREGADO
				token = "+";
			else if (pal == "-")//71  AGREGADO
				token = "-";
			else if (pal == "*")//72   
				token = "*";
			else if (pal == "/")//73  AGREGADO
				token = "/";
			else if (pal == ">")//81
				token = "r";
			else if (pal == "<")//82
				token = "r";
			else if (pal == "=")//83
				token = "r";
			else if (pal == ">=")//84
				token = "r";
			else if (pal == "<=")//85
				token = "r";
			else if (!pal.Contains('%') && !pal.Contains(',') && !pal.Contains('(') && !pal.Contains(')') && !pal.Contains('*') && !pal.Contains('<') && !pal.Contains('>') && !pal.Contains('='))
				token = "i";
			lislex.Add(token);
		}

		bool MMIQ(DataGridView dgv, ref int column, string tokenAnt, ref int cNum)///
		{
			if (tokenAnt == ">" || tokenAnt == "<")
			{
				colum--;
				cNum--;
				return true;
			}
			else
				return false;
		}


        void ValidarEst(char car, int[,] Estados, ref int est, DataGridView dgv, List<string> lislex, List<string> lisErCLin)
        {
            char carac = ' ';
            if (car != ' ' && Char.IsLetter(car) && alfa == false)
            {
                x = 0;
                tipo = 100;
            }
            else if (car != ' ' && Char.IsNumber(car) && alfa == false)
            {
                x = 1;
                if (est != 1 && est != 2)
                    tipo = 62;
            }
            else if (car != ' ' && alfa == false && coincide(delim, car) == true && Convert.ToString(car) != apost)
            {
                x = 2;
                tipo = 5;//delimitadores
            }
            else if (car != ' ' && alfa == false && coincide(oper, car) == true)
            {
                x = 3;
                tipo = 7;//operadores
            }
            else if (car != ' ' && alfa == false && coincide(rela, car) == true)
            {
                x = 6;
                tipo = 8;//relacionales
            }
            else if (car != ' ' && Convert.ToString(car) == apost)//alfanumerico
            {
                x = 4;//valor=36
                calfa++;
                tipo = 5;//alfanumericos y delimitadores NO constantes
                if (calfa == 1)
                    alfa = true;
                else
                {
                    alfa = false;
                    calfa = 0;
                }

            }
            else if (car != ' ' && (car == '#' || car == '_') && alfa == false)
            {
                x = 5;
                tipo = 100;//4  identificadores
            }
            else if (car != ' ' && car == '\n' && alfa == false)//con \n
            {
                if (pal != "")
                {
                    QtipPal(pal, palRes, ref tipo);//**

                    //agregar al arreglo de Identificadores
                    if (!coincidepal(palRes, pal) && !coincidepal(delim, pal) && !coincidepal(oper, pal) && !coincidepal(rela, pal) && tipo != 5 && tipo != 62)//Falta relacionales***
                        LlenarArreglo(pal, ref viden, ref ciden, ref iden, ref tipo, ref codidco);
                    else if (pal != ";" && !coincidepal(iden, pal) && !coinciapost(pal) && (tipo == 62 || tipo == 5))
                        LlenarArreglo(pal, ref vcons, ref ccons, ref cons, ref tipo, ref codidco);

                    //else if (!coincidepal(iden, pal) && !coinciapost(pal) && (tipo == 62 || tipo == 5))//tipo=5 constantes
                    if (tipo != 62)
                    {
                        cNum++;
                        dgv.RowCount++;
                        dgv[cdgv, colum].Value = cNum;
                        cdgv++;
                        dgv[cdgv, colum].Value = cLin;
                        cdgv++;
                        dgv[cdgv, colum].Value = pal;
                        cdgv++;
                        dgv[cdgv, colum].Value = tipo;
                        cdgv++;

                        if (idco == false)
                        {
                            dgv[cdgv, colum].Value = codificacion(pal, ref tipo);
                        }
                        else
                        {
                            dgv[cdgv, colum].Value = codidco;
                            idco = false;
                        }

                        cdgv = 0;
                        colum++;
                        AgregarTokens(lislex, pal);
                        lisErCLin.Add(cLin.ToString());
                    }
                    else
                    {
                        carac = pal[0];
                        if (Convert.ToString(carac) == "'")
                        {
                            //'
                            cNum++;
                            dgv.RowCount++;
                            dgv[cdgv, colum].Value = cNum;
                            cdgv++;
                            dgv[cdgv, colum].Value = cLin;
                            cdgv++;
                            lisErCLin.Add(cLin.ToString());//**********
                            dgv[cdgv, colum].Value = Convert.ToString(carac);
                            lislex.Add(Convert.ToString(carac));//**
                            cdgv++;
                            dgv[cdgv, colum].Value = 5;
                            cdgv++;
                            dgv[cdgv, colum].Value = 54;
                            cdgv = 0;
                            colum++;
                            //a
                            cNum++;
                            dgv.RowCount++;
                            dgv[cdgv, colum].Value = cNum;
                            cdgv++;

                            dgv[cdgv, colum].Value = cLin;
                            lisErCLin.Add(cLin.ToString());//*********
                            cdgv++;
                            dgv[cdgv, colum].Value = "CONSTANTE";
                            lislex.Add("a");
                            cdgv++;
                            dgv[cdgv, colum].Value = tipo;
                            cdgv++;
                            //dgv[cdgv, colum].Value = codificacion(pal, ref tipo);
                            dgv[cdgv, colum].Value = codidco;
                            cdgv = 0;
                            colum++;

                            //'
                            cNum++;
                            dgv.RowCount++;
                            dgv[cdgv, colum].Value = cNum;
                            cdgv++;
                            dgv[cdgv, colum].Value = cLin;
                            lisErCLin.Add(cLin.ToString());//****************
                            cdgv++;
                            dgv[cdgv, colum].Value = Convert.ToString(carac);
                            lislex.Add(Convert.ToString(carac));
                            cdgv++;
                            dgv[cdgv, colum].Value = 5;
                            cdgv++;
                            dgv[cdgv, colum].Value = 54;
                            cdgv = 0;
                            colum++;

                            idco = false;
                            //

                        }
                        else
                        {
                            cNum++;
                            dgv.RowCount++;
                            dgv[cdgv, colum].Value = cNum;
                            cdgv++;
                            dgv[cdgv, colum].Value = cLin;
                            lisErCLin.Add(cLin.ToString());//***********
                            cdgv++;
                            dgv[cdgv, colum].Value = "CONSTANTE";
                            lislex.Add("d");
                            cdgv++;
                            dgv[cdgv, colum].Value = tipo;
                            cdgv++;
                            //dgv[cdgv, colum].Value = codificacion(pal, ref tipo);
                            dgv[cdgv, colum].Value = codidco;
                            cdgv = 0;
                            colum++;
                            idco = false;
                        }

                    }



                    pal = "";
                    //cLin++;
                }
            }
            else if (car == ' ' && alfa == false) //esta es la seccion final***
            {
                if (pal != "")
                {
                    QtipPal(pal, palRes, ref tipo);//**
                                                   //agregar al arreglo de Identificadores
                    if (!coincidepal(palRes, pal) && !coincidepal(delim, pal) && !coincidepal(oper, pal) && !coincidepal(rela, pal) && tipo != 5 && tipo != 62)//62****
                        LlenarArreglo(pal, ref viden, ref ciden, ref iden, ref tipo, ref codidco);
                    else if (pal != ";" && !coincidepal(iden, pal) && !coinciapost(pal) && (tipo == 62 || tipo == 5))
                        LlenarArreglo(pal, ref vcons, ref ccons, ref cons, ref tipo, ref codidco);

                    if (tipo != 62)
                    {
                        //aqui cae M
                        cNum++;
                        dgv.RowCount++;
                        dgv[cdgv, colum].Value = cNum;
                        cdgv++;
                        dgv[cdgv, colum].Value = cLin;
                        cdgv++;
                        dgv[cdgv, colum].Value = pal;
                        cdgv++;
                        dgv[cdgv, colum].Value = tipo;
                        cdgv++;
                        dgv.Refresh();
                        //Aqui hay que hacer algo con la codificacion de constantes e identificadores con una bool******
                        if (idco == false)
                        {
                            dgv[cdgv, colum].Value = codificacion(pal, ref tipo);
                        }
                        else
                        {
                            dgv[cdgv, colum].Value = codidco;
                            idco = false;
                        }

                        cdgv = 0;
                        colum++;
                        AgregarTokens(lislex, pal);
                        lisErCLin.Add(cLin.ToString());//********************
                    }
                    else
                    {

                        carac = pal[0];
                        if (Convert.ToString(carac) == "'")
                        {
                            //'
                            cNum++;
                            dgv.RowCount++;
                            dgv[cdgv, colum].Value = cNum;
                            cdgv++;
                            dgv[cdgv, colum].Value = cLin;
                            lisErCLin.Add(cLin.ToString());//*********************
                            cdgv++;
                            dgv[cdgv, colum].Value = Convert.ToString(carac);
                            lislex.Add(Convert.ToString(carac));
                            cdgv++;
                            dgv[cdgv, colum].Value = 5;
                            cdgv++;
                            dgv[cdgv, colum].Value = 54;
                            cdgv = 0;
                            colum++;
                            //a
                            cNum++;
                            dgv.RowCount++;
                            dgv[cdgv, colum].Value = cNum;
                            cdgv++;

                            dgv[cdgv, colum].Value = cLin;
                            lisErCLin.Add(cLin.ToString());//****************
                            cdgv++;
                            dgv[cdgv, colum].Value = "CONSTANTE";
                            lislex.Add("a");
                            cdgv++;
                            dgv[cdgv, colum].Value = tipo;
                            cdgv++;
                            //dgv[cdgv, colum].Value = codificacion(pal, ref tipo);//***codidco
                            dgv[cdgv, colum].Value = codidco;
                            cdgv = 0;
                            colum++;

                            //'
                            cNum++;
                            dgv.RowCount++;
                            dgv[cdgv, colum].Value = cNum;
                            cdgv++;
                            dgv[cdgv, colum].Value = cLin;
                            lisErCLin.Add(cLin.ToString());//*********************
                            cdgv++;
                            dgv[cdgv, colum].Value = Convert.ToString(carac);
                            lislex.Add(Convert.ToString(carac));
                            cdgv++;
                            dgv[cdgv, colum].Value = 5;
                            cdgv++;
                            dgv[cdgv, colum].Value = 54;
                            cdgv = 0;
                            colum++;
                            //
                            idco = false;
                        }
                        else
                        {
                            cNum++;
                            dgv.RowCount++;
                            dgv[cdgv, colum].Value = cNum;
                            cdgv++;
                            dgv[cdgv, colum].Value = cLin;
                            lisErCLin.Add(cLin.ToString());//**********************
                            cdgv++;
                            dgv[cdgv, colum].Value = "CONSTANTE";
                            lislex.Add("d");
                            cdgv++;
                            dgv[cdgv, colum].Value = tipo;
                            cdgv++;
                            //dgv[cdgv, colum].Value = codificacion(pal, ref tipo);
                            dgv[cdgv, colum].Value = codidco;
                            cdgv = 0;
                            colum++;

                            // cLin++;
                            idco = false;
                        }
                    }

                    pal = "";

                    //  x = 0;
                }
            }
            else if (car != ' ' && car != '\n' && car != '\t' && !Char.IsLetter(car) && !Char.IsNumber(car) && alfa == false)// Errores
            {
                lisError.Add("Error 1:101 Línea " + cLin + " Símbolo desconocido " + car);
                error = true;
                return;
            }


            if (car == '\n')
                cLin++;
            apunEsc = cLin;

            if (car != '=' && car != ' ' && car != '\n')
            {
                //
                pal += car; //pal selectselectselect
                tokenAnt = pal;
            }
            else if (car == '=' && car != ' ' && car != '\n')
            {
                if (MMIQ(dgv, ref colum, tokenAnt, ref cNum))
                {
                    pal = tokenAnt + '=';
                    tokenAnt = pal;
                }
                else
                {
                    pal += car;
                    tokenAnt = pal;
                }
            }


            if (alfa == false)
                est = Estados[est, x];
            else if (alfa == true && calfa == 1)
            {
                est = Estados[est, x];
                calfa++;
            }
            else if (alfa == true && car == ' ')
            {
                pal += car;
                tokenAnt = pal;
            }
        }

		private bool parse()//Algoritmo LL
		{
			bool error = false;
			pila.Push("$");
			pila.Push("Q");
			string x = "";
			string k = "";
			int apuntador = 0;
			int col = 0, fila = 0;
			string kant = "";
			string palResANt = "";

			do
			{
				if (apuntador == 6)
				{

				}
				x = pila.Pop();
				k = lislex[apuntador];

				if (apuntador > 0)
					kant = lislex[apuntador - 1];

				if (kant != "" && kant != "o" && kant != "y" && coincidepalErr(palRes, kant))
					palResANt = kant;

				if (EsTerminalDML(x))//Esterminal(x)
				{
					if (x == k)
					{
						apuntador++;
					}
					else
					{
						error = true;
						ErroresXK(x, k, apuntador, lisErCLin);
						break;
					}
				}
				else
				{
					valorDML(x, k, ref col, ref fila);

					string au = "";
					au = matriz2[col, fila];
					//16,6
					string strng = matriz2[col, fila];
					if (strng != null)
					{
						if (matriz2[col, fila] != "0")
						{
							Inversamente(matriz2, col, fila, pila);
						}
					}
					else
					{
						error = true;
						ErroresMatriz(x, k, kant, apuntador, lisErCLin, palResANt);
						break;
					}

				}

			} while (x != "$");

			if (x == "$" && error == false)
				setStatus("Cadena válida");
			return !error;
		}

		private bool ddl()//Algoritmo LL
		{
			bool error = false;
			string PrimSig = "";
			string x = "";
			string k = "";
			int apuntador = 0;
			int col = 0, fila = 0;
			string kant = "";


			PrimSig = k = lislex[apuntador];
			pila.Push("$");

			if (PrimSig == "c")
				pila.Push("C");
			else if (PrimSig == "m")
				pila.Push("I");
			do
			{
				if (apuntador == 33)
				{

				}
				x = pila.Pop();
				k = lislex[apuntador];

				if (apuntador > 0)
					kant = lislex[apuntador - 1];

				if (x == "t" || x == "g" || x == "k" || x == "d" || x == "q" || x == "v" || x == "a" || x == ";" || EsTerminalDDL(x))//Esterminal(x)//***
				{
					if (x == k)
					{
						apuntador++;
					}
					else
					{
						error = true;
						ErroresXKddl(x, k, apuntador, kant, lisErCLin);
						break;
					}
				}
				else
				{
					valorDDL(x, k, ref col, ref fila, apuntador);

					string au = "";
					au = DDL[col, fila];
					if (DDL[col, fila] != null)
					{
						if (DDL[col, fila] != "0")
						{
							Inversamente(DDL, col, fila, pila);
						}
					}
					else
					{
						error = true;
						ErroresMatrizddl(x, k, kant, apuntador, lisErCLin);
						break;
					}

				}

			} while (x != "$");

			if (x == "$" && error == false)
				setStatus("Cadena válida");
			return !error;
		}

		bool EsTerminalDDL(string x)
		{
			return new[] { "c", "m", "h", "u", "e", ",", "i", "b", "p", "j", "l", "'", "d", ")", "(", "$" }.Contains(x);
		}

        void ErroresXKddl(string x, string k, int apuntador, string kant, List<string> lisErCLin)
        {
            int apu = 0;

            if (apuntador < lisErCLin.Count)
            {
                apu = apuntador;
            }
            else
                apu = lisErCLin.Count - 1;

            apunEsc = apu;
            if ((x == "q" && k == "i" && kant == "m") || (x == "k" && k == "i" && kant == "j") || (kant == "" && k == "i" && x == "$") || (x == "v" && k == "i" && kant == "i"))
            {
                setStatus("Error 2:201 Línea " + lisErCLin[apu] + " El tipo de dato " + DgvData[2, apuntador].Value.ToString() + " no es válido.");
                return;
            }
            else if ((x == "(" && k == "i" && kant == "i") || (x == "(" && k == "i" && kant == "v") || (x == "(" && k == "'" && kant == "v") || (x == "(" && k == "i" && kant == "h") || (x == "(" && k == "i" && kant == "u") || (x == "(" && k == "i" && kant == "k"))
            {
                setStatus("Error 2:202 Línea " + lisErCLin[apu] + " Falta el delimitador “(”.");
                return;
            }
            else if ((x == ")" && k == "e" && kant == "d") || (x == ")" && k == "e" && kant == "i") || (x == ")" && k == "$" && kant == "i"))
            {
                setStatus("Error 2:202 Línea " + lisErCLin[apu] + " Falta el delimitador “)”.");
                return;
            }
            else if ((x == ";" && k == "m" && kant == ")") || (x == ";" && k == "$" && kant == ")"))
            {
                setStatus("Error 2:202 Línea " + lisErCLin[apu] + " Falta el delimitador “;”.");
                return;
            }
            else if ((x == "i" && k == ")" && kant == "(") || (x == "i" && k == "p" && kant == "b") || (x == "i" && k == "j" && kant == "b") || (x == "i" && k == "v" && kant == "q"))
            {
                setStatus("Error 2:203 Línea " + lisErCLin[apu] + " Falta atributo.");
                return;
            }
            else if ((x == "$" && k == "q" && kant == "") || (x == "k" && k == "(" && kant == "p") || (x == "v" && k == "(" && kant == "i") || (x == "k" && k == "(" && kant == "j") || (x == "$" && k == "t" && kant == ""))
            {
                setStatus("Error 2:205 Línea " + lisErCLin[apu] + " Falta palabra reservada.");
                return;
            }
            else if (x == "d" && k == "i" && kant == "(")
            {
                setStatus("Error 2:201 Línea " + lisErCLin[apu] + " El tipo de dato " + DgvData[2, apuntador].Value.ToString() + " no es válido.");
                return;
            }
            else if (x == "(" && k == "$" && kant == "i")
            {
                setStatus("Error 2:202 Línea " + lisErCLin[apu] + " Falta el delimitador “(”.");
                return;
            }
            else if (x == "g" && k == "i" && kant == "e")
            {
                setStatus("Error 2:205 Línea " + lisErCLin[apu] + " Falta palabra reservada.");
                return;
            }
            else if (x == "t" && k == "i" && kant == "c")
            {
                setStatus("Error 2:205 Línea " + lisErCLin[apu] + " Falta palabra reservada.");
                return;
            }
            else if (x == "k" && k == "i" && kant == "p")
            {
                setStatus("Error 2:205 Línea " + lisErCLin[apu] + " Falta palabra reservada.");
                return;
            }
            else if (x == "(" && k == "," && kant == "i")
            {
                setStatus("Error 2:202 Línea " + lisErCLin[apu] + " Falta el delimitador “(”.");
                return;
            }
            else if (x == ")" && k == "," && kant == "i")
            {
                setStatus("Error 2:202 Línea " + lisErCLin[apu] + " Falta el delimitador “)”.");
                return;
            }
            else if (x == "i" && k == "," && kant == "(")
            {
                setStatus("Error 2:203 Línea " + lisErCLin[apu] + " Falta atributo");
                return;
            }
            else if (x == "k" && k == "," && kant == "p")
            {
                setStatus("Error 2:205 Línea " + lisErCLin[apu] + " Falta palabra reservada");
                return;
            }
            else if (x == "i" && (k == "," || k == "." || k == "(" || k == ")" || k == "'") && kant == "b")
            {
                setStatus("Error 2:203 Línea " + lisErCLin[apu] + " Falta atributo");
                return;
            }
            else if (x == "t" && (k == "," || k == "." || k == "(" || k == ")" || k == "'") && kant == "c")
            {
                setStatus("Error 2:205 Línea " + lisErCLin[apu] + " Falta palabra reservada");
                return;
            }
            else if (x == "i" && k == "," && kant == "t")
            {
                setStatus("Error 2:203 Línea " + lisErCLin[apu] + " Falta atributo");
                return;
            }
            else if (x == "(" && (k == "," || k == "." || k == ")" || k == "'") && kant == "h")
            {
                setStatus("Error 2:202 Línea " + lisErCLin[apu] + " Falta el delimitador “(”.");
                return;
            }
            else if (x == "d" && k == "," && kant == "(")
            {
                setStatus("Error Línea " + lisErCLin[apu] + " Hace falta constante");
                return;
            }
            else if (x == ")" && (k == "," || k == "." || k == "(" || k == "'" || k=="$") && kant == "d")
            {
                setStatus("Error 2:202 Línea " + lisErCLin[apu] + " Falta el delimitador “)”.");
                return;
            }
            else if (x == "g" && k == "," && kant == "e")
            {
                setStatus("Error 2:205 Línea " + lisErCLin[apu] + " Falta palabra reservada");
                return;
            }
            else if (x == "d" && k == ")" && kant == "(")
            {
                setStatus("Error 2:202 Línea " + lisErCLin[apu] + " Hace falta constante");
                return;
            }
            else if (x == "(" && k == "," && kant == "k")
            {
                setStatus("Error 2:202 Línea " + lisErCLin[apu] + " Falta el delimitador “(”.");
                return;
            }
            else if (x == "t" && k == "$" && kant == "c")//OC
            {
                setStatus("Error 2:205 Línea " + lisErCLin[apu] + " Falta palabra reservada.");
                return;
            }
            else if (x == "i" && (k == "," || k == "." || k == "(" || k == ")" || k == "'") && kant == "t")
            {
                setStatus("Error 2:203 Línea " + lisErCLin[apu] + " Falta atributo");
                return;
            }
            else if (x == "(" && (k == "," || k == "." || k == "(" || k == ")" || k == "'") && kant == "i")
            {
                setStatus("Error 2:202 Línea " + lisErCLin[apu] + " Falta el delimitador “(”.");
                return;
            }
            else if (x == "A" && (k == "," || k == "." || k == "(" || k == ")" || k == "'") && kant == "(")
            {
                setStatus("Error 2:202 Línea " + lisErCLin[apu] + " Falta el delimitador “(”.");
                return;
            }
            else if (x == "d" && k == "(" && kant == "(")
            {
                setStatus("Error 2:202 Línea " + lisErCLin[apu] + " Hace falta constante");
                return;
            }
            else if (x == ")" && k == "$" && kant == "d")
            {
                setStatus("Error 2:202 Línea " + lisErCLin[apu] + " Hace falta constante");
                return;
            }
            else
            {
                LblStatus.Text += ".";
                return;
            }
        }

		void ErroresMatrizddl(string x, string k, string kant, int apuntador, List<string> lisErCLin)
		{
			int apu = 0;


			if (apuntador < lisErCLin.Count)
			{
				apu = apuntador;
			}
			else
				apu = lisErCLin.Count - 1;
			apunEsc = apu;

			if ((kant == "i" && k == "i" && x == "D") || (kant == ";" && k == "i" && x == "N") || (kant == "," && k == "i" && x == "H"))
			{
				setStatus("Error 2:201 Línea " + lisErCLin[apu] + " El tipo de dato " + DgvData[2, apuntador].Value.ToString() + " no es válido.");
				return;
			}
			else if ((kant == ")" && k == "$" && x == "K") || (kant == ")" && k == "$" && x == "L"))//
			{
				setStatus("Error 2:202 Línea " + lisErCLin[apu] + " Falta el delimitador “)”.");
				return;
			}
			else if ((kant == "i" && k == "'" && x == "E") || (k == "$" && kant == "i" && x == "E") || (kant == "r" && k == "$" && x == "O"))
			{
				setStatus("Error 2:202 Línea " + lisErCLin[apu] + " Falta el delimitador apostrofo.");
				return;
			}
			else if ((kant == "'" && k == "'" && x == "P") || (kant == "'" && k == "d" && x == "P") || (kant == "d" && k == "'" && x == "P") || (kant == ")" && k == "b" && x == "K") || (kant == ")" && k == "b" && x == "L"))
			{
				setStatus("Error 2:202 Línea " + lisErCLin[apu] + " Falta delimitador.");
				return;
			}
			else if ((kant == "," && k == ")" && x == "M") || (kant == "(" && k == "h" && x == "A") || (kant == "," && k == "h" && x == "G") || (kant == "," && k == "u" && x == "G") || (kant == "(" && k == "," && x == "M") || (kant == "," && k == "," && x == "M") || (kant == "," && k == ")" && x == "M"))
			{
				setStatus("Error 2:203 Línea " + lisErCLin[apu] + " Falta atributo.");
				return;
			}
			else if ((kant == ")" && k == "i" && x == "K") || (kant == "i" && k == "k" && x == "J") || (kant == "i" && k == "p" && x == "D") || (kant == ";" && k == "q" && x == "N"))
			{
				setStatus("Error 2:205 Línea " + lisErCLin[apu] + " Falta palabra reservada.");
				return;
			}
			else if (kant == ")" && k == "$" && x == "E")
			{
				setStatus("Sintaxis incorrecta");
				return;
			}
			else if (kant == "," && k == ")" && x == "G")
			{
				setStatus("Error 2:203 Línea " + lisErCLin[apu] + " Falta atributo.");
				return;
			}
			else if (kant == ")" && k == "i" && x == "E")
			{
				setStatus("Error 2:205 Línea " + lisErCLin[apu] + " Falta palabra reservada.");
				return;
			}
			else if (kant == "g" && k == "i" && x == "F")
			{
				setStatus("Error 2:205 Línea " + lisErCLin[apu] + " Falta palabra reservada.");
				return;
			}
			else if (kant == "i" && k == "i" && x == "J")
			{
				setStatus("Error 2:205 Línea " + lisErCLin[apu] + " Falta palabra reservada.");
				return;
			}
			else if (kant == "i" && k == "," && x == "J")
			{
				setStatus("Error 2:203 Línea " + lisErCLin[apu] + " Falta palabra reservada.");
				return;
			}
            else if (kant == "," && (k == "e") && x == "G")
            {
                setStatus("Error 2:203 Línea " + lisErCLin[apu] + " Falta atributo.");
                return;
            }
            else if (kant == ")" && (k == "e") && x == "B")
            {
                setStatus("Error 2:203 Línea " + lisErCLin[apu] + " Falta atributo.");
                return;
            }
            else if (kant == ")" && k == ")" && x == "B")
            {
                setStatus("Error 2:205 Línea " + lisErCLin[apu] + " Falta palabra reservada.");
                return;
            }
            else if (kant == "c" && (k == "," || k == "." || k == "(" || k == ")" || k == "'" || k == "$") && x == "t")
            {
                setStatus("Error 2:205 Línea " + lisErCLin[apu] + " Falta palabra reservada.");
                return;
            }
            else if (kant == "(" && (k == "," || k == "." || k == "(" || k == ")" || k == "'" || k == "$") && x == "A")
            {
                setStatus("Error 2:203 Línea " + lisErCLin[apu] + " Falta atributo.");
                return;
            }else if (kant == "i" && k == "$" && x == "D")
            {
                setStatus("Error 2:201 Línea " + lisErCLin[apu] + " El tipo de dato \" ' \" no es válido.");
                return;
            }
            else if (kant == "i" && (k == "," || k == "." || k == "(" || k == ")" || k == "'") && x == "D")
            {
                setStatus("Error 2:201 Línea " + lisErCLin[apu] + " El tipo de dato \"" + DgvData[2, apuntador].Value.ToString() + "\" no es válido.");
                return;
            }
            
            else
            {
                LblStatus.Text += ".";
                return;
            }
		}

		void setStatus(string status)
		{
			LblStatus.Text = status;
		}

		#region General Form Events

		private void RtxtSynt_KeyPress(object sender, KeyPressEventArgs e)
		{
			try
			{
				if (!string.IsNullOrWhiteSpace(RichTxt.Text))
				{
					if (e.KeyChar == ' ' || e.KeyChar == (char)Keys.Enter || e.KeyChar == (char)Keys.Back)
					{
						//shortScann1(e);
					}
				}
			}
			catch
			{

			}
			e.KeyChar = (e.KeyChar.ToString()).ToUpper().ToCharArray()[0];

		}

		private void BtnClose_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void BtnMaximize_Click(object sender, EventArgs e)
		{
			this.BackColor = (WindowState == FormWindowState.Maximized) ? Color.DodgerBlue : Color.DimGray;
			WindowState = (WindowState == FormWindowState.Maximized) ? FormWindowState.Normal : FormWindowState.Maximized;
		}

		private void BtnMinimize_Click(object sender, EventArgs e)
		{
			WindowState = FormWindowState.Minimized;
		}

		private void BtnClosePanel_Click(object sender, EventArgs e)
		{
			PanelCtrl();
		}

		private void BtnOpenResults_Click(object sender, EventArgs e)
		{
			PanelCtrl();
		}

		private void PnlTitle_MouseDown(object sender, MouseEventArgs e)
		{
			MovingForm(e);
		}

		private void LblTitle_MouseDown(object sender, MouseEventArgs e)
		{
			MovingForm(e);
		}

		private void PbxLogo_MouseDown(object sender, MouseEventArgs e)
		{
			MovingForm(e);
		}

		private void PnlOptions_MouseDown(object sender, MouseEventArgs e)
		{
			MovingForm(e);
		}

		private void TxtGuide_MouseDown(object sender, MouseEventArgs e)
		{
			MovingForm(e);
		}

		private void PnlBtnResults_MouseDown(object sender, MouseEventArgs e)
		{
			MovingForm(e);
		}

		private void PnlResults_MouseDown(object sender, MouseEventArgs e)
		{
			MovingForm(e);
		}

		private void TxtGuide_Enter(object sender, EventArgs e)
		{
			ActiveControl = RichTxt;
		}

		#endregion General Form Events

		#region General Form Methods


		private void Builder()
		{


			int i = 0;
			while (true)
			{
				i++;
				TxtGuide.AppendText(i.ToString());
				TxtGuide.AppendText(Environment.NewLine);
				if (i == 500)
					break;
			}
			RichTxt.Select();

			resArray = System.IO.File.ReadAllLines(@"..\\..\\Docs\\tsql_1_reserved.txt");
			delArray = System.IO.File.ReadAllText(@"..\\..\\Docs\\tsql_2_delimiter.txt").ToCharArray().Select(c => c.ToString()).ToArray();
			opeArray = System.IO.File.ReadAllText(@"..\\..\\Docs\\tsql_4_operators.txt").ToCharArray().Select(c => c.ToString()).ToArray();
			conArray = System.IO.File.ReadAllText(@"..\\..\\Docs\\tsql_3_constant.txt").ToCharArray().Select(c => c.ToString()).ToArray(); //?
			relArray = System.IO.File.ReadAllLines(@"..\\..\\Docs\\tsql_5_relational.txt"); //?
			mainArrays = new Array[] { resArray, delArray, opeArray, conArray, relArray };
		}

		void PanelCtrl()
		{
			PnlBtnResults.Visible ^= true;
			PnlResults.Visible ^= true;
		}

		void MovingForm(MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				ReleaseCapture();
				SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
			}
		}

		void shortScann1(KeyPressEventArgs e)
		{
			if (RichTxt.Text.Substring(RichTxt.SelectionStart - 1, 1) != ' '.ToString())
			{
				int wordEndPosition = RichTxt.SelectionStart;
				int currentPosition = wordEndPosition;

				while (currentPosition > 0 && RichTxt.Text[currentPosition - 1] != ' ' && RichTxt.Text[currentPosition - 1] != '\n' && RichTxt.Text[currentPosition - 1] != '\'' && !char.IsPunctuation(RichTxt.Text[currentPosition - 1]))//RichTxt.Text[currentPosition - 1] != e.KeyChar)
				{
					currentPosition--;
				}

				//if (e.KeyChar == (char)Keys.Enter)
				//{
				//    wordEndPosition--;
				//}

				string word = RichTxt.Text.Substring(currentPosition, wordEndPosition - currentPosition);
				Color color;

				if (e.KeyChar == (char)Keys.Back)
				{
					color = Color.WhiteSmoke;  // ?
				}
				else
				{
					color = null != Scann(word, false) ? Color.FromArgb(0, 200, 255) : (null != Scann(word, false) ? Color.LightSalmon : (null != Scann(word, false) ? Color.LightGray : Color.WhiteSmoke));
				}

				wordColor(color, currentPosition, wordEndPosition);
			}
		}

		String[] Scann(string word, bool count)
		{
			int? i = 0, i2 = 0;
			string typeDescrpt = null;
			cntArray = 0;
			foreach (Array coll in mainArrays)
			{
				i = cntArray == 0 ? 10 : cntArray == 1 ? 50 : cntArray == 2 ? 70 : cntArray == 3 ? 60 : cntArray == 4 ? 80 : 0;
				typeDescrpt = cntArray == 0 ? "Palabra reservada" : cntArray == 1 ? "Delimitador" : cntArray == 2 ? "Operador" : cntArray == 3 ? "Constante" : cntArray == 4 ? "Relacional" : "?";
				i2 = 0;

				foreach (string s in coll)
				{
					if (s == word)
					{
						return new string[] { (i + i2).ToString(), typeDescrpt };
					}
					i2++;
				}
				cntArray++;
			}
			if (count)
				return
					flagConstant || flagConstantNum ?
						new string[] { valConst(word), "Constante" } :
					(!word.Contains('%') && !word.Contains('|') && !word.Contains('°') && !word.Contains('¬') && !word.Contains('~') && !word.Contains('\\')) ?
						new string[] { valIdent(word), "Identificador" } :

						new string[] { valDesc(word), "Desconocido" };
			else
				return null;
		}

		private string valConst(string word)
		{
			return constValue++.ToString();
		}

		private string valIdent(string word)
		{
			return identValue++.ToString();
		}

		private string valDesc(string word)
		{
			return "0";
		}

		void wordColor(Color color, int currentPosition, int wordEndPosition)
		{
			RichTxt.Select(currentPosition, wordEndPosition - currentPosition);
			RichTxt.SelectionColor = color;
			RichTxt.Select(wordEndPosition, wordEndPosition);
			RichTxt.SelectionColor = Color.WhiteSmoke;
			//RichTxt.Select(RichTxt.TextLength, RichTxt.TextLength);
		}
		#endregion General Form Methods

		private void TxtRgx_KeyUp(object sender, KeyEventArgs e)
		{
			try
			{
				File.WriteAllText(@"..\\..\\Docs\\Regex.txt", TxtRgx.Text);
			}
			catch (Exception E)
			{
				MessageBox.Show(E.Message);
			}
		}


		#region General Form Instances

		private string[] resArray, delArray, opeArray, conArray, relArray;
		private string[,] matrix;
		private int lastCount = 0, count = 0, status = 0, calls, tokens, identifi, constant, lastStatus;
		private char car;
		private bool printed, error;
		private string token = "";
		private Array[] mainArrays;
		private DataTable idCons = new DataTable(), reserv = new DataTable();
		private static bool flagConstant, flagConstantNum;
		private static int constValue = 63, identValue = 101, cntArray = 0;


		public const int WM_NCLBUTTONDOWN = 0xA1;
		public const int HT_CAPTION = 0x2;

		[DllImportAttribute("user32.dll")]
		public static extern int SendMessage(IntPtr hWnd,
						 int Msg, int wParam, int lParam);
		[DllImportAttribute("user32.dll")]
		public static extern bool ReleaseCapture();

        #endregion General Form Instances

        #region Private Instances

        Stack<string> pila;
        List<string> lislex; //= new List<string>();
        List<string> lisError;
        List<string> lisErCLin;
        bool prse = true, alfa = false, idco = false;
        string pal = "", apost = "'", tokenAnt = "", cad = "";
        int x = 0, tipo = 0, calfa = 0, codidco = 0, cdgv = 0, colum = 0, ciden = 0, viden = 101, ccons = 0,
            vcons = 63, cNum = 0, cLin = 1, apunEsc = 0, est = 0, cont = 0;

        char[] Aest, Aux;

        string[,] palRes ={{"SELECT","10","s"},
                         {"FROM","11","f"},
                         {"WHERE","12","w"},
                         {"IN","13","n"},
                         {"AND","14","y"},
                         {"OR","15","o"},
                         {"CREATE","16","c"},
                         {"TABLE","17","t"},
                         {"CHAR","18","h"},
                         {"NUMERIC","19","u"},
                         {"NOT","20","e"},
                         {"NULL","21","g"},
                         {"CONSTRAINT","22","b"},
                         {"KEY","23","k"},
                         {"PRIMARY","24","p"},
                         {"FOREIGN","25","j"},
                         {"REFERENCES","26","l"},
                         {"INSERT","27","m"},
                         {"INTO","28","q"},
                         {"VALUES","29","v"}};

        string[,] delim ={{",","50",","},
                         {".","51","."},
                         {"(","52","p"},
                         {")","53","p"},
                         {"'","54","'"},
                         {";","55",";"}};

        string[,] rela ={{">","81","r"},
                        {"<","82","r"},
                        {"=","83","r"},
                        {">=","84","r"},
                        {"<=","85","r"}};

        string[,] oper ={{"+","70","+"},
                      {"-","71","-"},
                      {"*","72","*"},
                      {"/","73","/"}};

        string[,] iden;

        string[,] cons;

        int[,] Estados ={   {1,7,3,4,5,0,8},//0
							{1,1,0,0,0,2,0},//1 letra/digito
							{0,0,0,0,0,0,0},//2 #
							{0,0,0,0,0,0,0},//3 delimitador
							{0,0,0,0,0,0,0},//4 operador
							{5,5,5,5,6,5,5},//5 ' inicio
							{0,0,0,0,0,0,0},//6 ' fin
							{0,7,0,0,0,0,0},//7 numero/digito
							{0,0,0,0,0,0,0}};//8 relacional
        #endregion Private Instances


        private Regex Rgx = new Regex(@"([_#a-zA-Z][_#a-zA-Z0-9]*)|(\b\d+)|(\')([^']*)(\'?)|([\.\,\(\)])|(>=|<=|<>|<|>|<|=)|([\+\*\/\-])");
		private bool IsValid(string value)
		{
			return Regex.IsMatch(value, TxtRgx.Text);
		}

	}

}