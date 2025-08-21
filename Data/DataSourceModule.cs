//---------------------------------------------------------------------------------------------------------------
// Skilful. Tactica Adversa graphic and analysis tool.
// Copyright (C) 2010 "...", Andrey Zyablitsev (skat)
//
// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License
// as published by the Free Software Foundation; either version 2
// of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
//
// mail: support@protoforma.com 
//---------------------------------------------------------------------------------------------------------------


using System;
using System.IO;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Skilful.QuotesManager;


namespace Skilful.Data
{
    public enum dm //перечисление предопределенных функций длл
    {
        SetTickHandler,
        RemTickHandler,
        import,
        init,
        get_symbol_list,
        get_symbol_count,
        get_history_length,
        get_tick,
        get_bar,
        get_pip_value,
        storagePath,
        ShowDialog,
        GetWorkingDir,
        SetWorkingDir,
        count
    }
    public enum evt
    {
        new_tick,
        new_bar,
        count
    }
    public enum LoginType { SelectFile, SelectDir, SetIPLoginPassword, UserDefinedForm }
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~//
    public class DataSourseModule
    {
        public Object inst;
        Dictionary<string, int> symslen;
        public string name, prompt;
        public bool enabled;

        //преобразование С_строки заканчивающейся 0 из массива char в строку С#
        static string c2a(char[] s)
        {
            int cnt = 0;
            while (s[cnt++] != '\0' && s.Length > cnt) ;
            return new string(s, 0, cnt - 1);
        }
        public MethodInfo[] method = new MethodInfo[(int)dm.count];
        EventInfo[] events = new EventInfo[(int)evt.count];

        public EventInfo Event(evt i)
        {
            return events[(int)i];
        }

        public void SetMethod(dm i, MethodInfo ptr)
        {
            method[(int)i] = ptr;
        }
        public void SetEvent(evt i, EventInfo ei)
        {
            events[(int)i] = ei;
        }
        public dm methods_count()
        {
            for (int i = 0; i < (int)dm.count; i++)
            {
                if (method[i] == null) return (dm)i;
            }
            return dm.count;
        }
        public bool validate_lib()
        {
            //как минимум 4 метода должены быть реализованы в длл
            return method[(int)dm.init] != null
                && method[(int)dm.import] != null
                && method[(int)dm.get_symbol_list] != null
                && method[(int)dm.get_symbol_list].GetParameters().Length == 3
                && method[(int)dm.get_symbol_count] != null;
        }

        /// <summary>
        /// функция предопределена для передачи тиков и баров в реальном времени
        /// вызов автоматически при создании объекта TSymbol в Skilful.QuotesManager::GetSymbol()
        /// </summary>
        /// <param name="sym">имя инструмента</param>
        public void SetTickHandler(string sym)
        {
            if (method[(int)dm.SetTickHandler] != null)
            {
                object[] args = { sym };
                method[(int)dm.SetTickHandler].Invoke(inst, args);
            }
        }

        /// <summary>
        /// отключение символа от реалтайм потока
        /// </summary>
        /// <param name="sym">имя инструмента</param>
        public void RemTickHandler(string sym)
        {
            if (method[(int)dm.RemTickHandler] != null)
            {
                object[] args = { sym };
                method[(int)dm.RemTickHandler].Invoke(inst, args);
            }
        }

        //импорт истории для данного символа, часовой фрейм
        public double[] import(string sym, int shift, int len)
        {
            int cnt = symslen[sym];
            if (cnt == 0) cnt = get_history_length(sym, TF.custom);
            if (len == 0 || len > cnt) len = cnt;
            if (len > 0)
            {
                double[] quotes = new double[len * 5];
                int[] len_ = { len };
                object[] args = { sym, TF.m60, shift, len_, quotes };

                method[(int)dm.import].Invoke(inst, args);

                //resize by real data count
                if (len_[0] < len) Array.Resize(ref quotes, len_[0] * 5);

                //trancate quotes array
                if (quotes[(len_[0] - 1) * 5] == 0)
                {
                    int i = (len_[0] - 1) * 5;
                    while (quotes[i] == 0 && i >= 5) i -= 5;
                    Array.Resize(ref quotes, i + 5);
                }
                return quotes;
            }
            else
                return null;
        }
        public double[] import(string sym)
        {
            return import(sym, 0, 0);
        }

        /// <summary>
        /// импорт истории для даного символа и фрейма, первоначально применяется для получения дневной истории в дополнение к часовой
        /// </summary>
        /// <param name="sym"></param>
        /// <param name="period"></param>
        /// <returns>массив double[] quotes, или null если в длл не определен метод get_history_length 
        /// или если история на данный символ+период недоступна</returns>
        public int get_history_length(string sym, TF tf)
        {
            if (method[(int)dm.get_history_length] == null) return 0;
            int[] len = {0};
            object[] args = { sym, tf, len };
            method[(int)dm.get_history_length].Invoke(inst, args);
            return len[0];
        }
        public double[] import(string sym, TF tf)
        {
            return import(sym, tf, 0, 0);
        }
        public double[] import(string sym, TF tf, int shift, int len)
        {
            int cnt = get_history_length(sym, tf);
            if (len == 0 || len > cnt) len = cnt;
            if (len > 0)
            {
                double[] quotes = new double[len * 5];
                int[] len_ = { len };
                object[] args = { sym, tf, shift, len_, quotes };

                method[(int)dm.import].Invoke(inst, args);

                //resize by real data count
                if (len_[0] < len) Array.Resize(ref quotes, len_[0] * 5);

                //trancate quotes array
                if (quotes[(len_[0] - 1) * 5] == 0)
                {
                    int i = (len_[0] - 1) * 5;
                    while (quotes[i] == 0 && i >= 5) i -= 5;
                    Array.Resize(ref quotes, i + 5);
                }
                return quotes;
            }
            else
                return null;
        }
        /// <summary>
        /// загрузка информации о модуле
        /// </summary>
        /// <param name="name">возвращает имя модуля</param>
        /// <param name="prompt">возвращает строку приглашения по умолчанию - например приглашение выбрать путь/файл/адрес провайдера, итп </param>
        /// <param name="description">первоначально предполагалась строка для всплывающей подсказки
        /// но сейчас используется в качестве фильтра/строки инициализации для опенФайл диалога
        /// или в качестве строки параметров и их значений по умолчанию для кастом диалога (пример: randlib.cs)</param>
        /// <param name="ltype">возвращает один из enum LoginType { SelectFile, SelectFolder, SetIPLoginPassword, UserDefined }</param>
        public void init(out string name, out string prompt, out string description, out LoginType ltype)
        {
            char[] cname = new char[256], cuserstring = new char[256], cdescription = new char[256];
            int[] cltype = {0};
            object[] args = { cname, cuserstring, cdescription, cltype };
            method[(int)dm.init].Invoke(inst, args);
            this.name = name = c2a(cname);
            this.prompt = prompt = c2a(cuserstring);
            description = c2a(cdescription);
            ltype = (LoginType)cltype[0];
        }
        public void init()
        {
            char[] cname = new char[256], cuserstring = new char[256], cdescription = new char[256];
            int[] cltype = {0};
            object[] args = { cname, cuserstring, cdescription, cltype };
            method[(int)dm.init].Invoke(inst, args);
            name = c2a(cname);
            prompt = c2a(cuserstring);
        }
        //чтение и инициализация файла
        /// <summary>
        /// возвращает список доступных инструментов
        /// </summary>
        /// <param name="filename">path+file</param>
        /// <param name="period">массив ТФ для каждого инструмента</param>
        /// <param name="len">массив длин истории каждого инструмента, для подготовки входных массивов функции импорта</param>
        /// <returns></returns>
        public string[] get_symbol_list(string filename, out int[] period, out int[] len)
        {
            //get_symbol_count
            int cnt = get_symbol_count(filename);

            string[] symbols = new string[cnt];
            len = new int[cnt];
            period = new int[cnt];

            object[] args = { symbols, period, len };
            method[(int)dm.get_symbol_list].Invoke(inst, args);

            symslen = new Dictionary<string, int>(cnt);
            int f = 0;
            for (int i = 0; i < cnt; i++)
            {
                if (symbols[i] != null)
                    symslen[symbols[i]] = len[i];
                else
                    f++;
            }
            if (f > 0)//some files not accessable, need compress list
            {
                int[] _len = len, _period = period;
                string[] _symbols = symbols;
                cnt -= f;
                symbols = new string[cnt];
                len = new int[cnt];
                period = new int[cnt];
                for (int i = 0, j = 0; i < cnt; i++)
                {
                    if (_symbols[i] != null)
                    {
                        symbols[j] = _symbols[i];
                        len[j] = _len[i];
                        period[j++] = _period[i];
                    }
                }
            }
            Array.Sort(symbols);
            return symbols;
        }
        /// <summary>
        /// возвращает список доступных инструментов
        /// </summary>
        /// <returns></returns>
        public string[] get_symbol_list(string filepath)
        {
            int[] period;
            int[] len;
            return get_symbol_list(filepath, out period, out len);
        }
        /// <summary>
        /// возвращает количество доступных символов
        /// </summary>
        /// <param name="filename">путь к файлу или папке, сохраняется в модуле для дальнейшего использования</param>
        /// <returns></returns>
        private int get_symbol_count(string filename)
        {
            int[] count = new int[1];
            object[] args = { filename, count };
            method[(int)dm.get_symbol_count].Invoke(inst, args);
            return count[0];
        }
        //следующие 2 метода могут вызываться менеджером данных с определенным интервалом
        //double[] tick == tick[0]=bid, tick[1]=ask, tick[2]=time(day.miliseconds)
        public void get_tick(string sym, double[] tick)
        {
            if (method[(int)dm.get_tick] != null)
            {
                object[] args = { sym, tick };
                method[(int)dm.get_tick].Invoke(inst, args);
            }
        }
        ////вход: если bar[0] не пустой - то должен хранить время последнего полученного бара
        ////если пустой то функция возвращает последний известный бар.
        public double[] get_bar(string sym, TF frame, DateTime lastTime, int lastPosition)
        {
            double[] bar = null;
            if (method[(int)dm.get_bar] != null)
            {
                bar = new double[5];
                object[] args = { sym, frame, lastTime, lastPosition };
                method[(int)dm.get_bar].Invoke(inst, args);
            }
            return bar;
        }

        //шаг изменения цены для данного инструмента
        public double get_pip_value(string sym)
        {
            double[] pip = new double[1];
            if (method[(int)dm.get_pip_value] != null)
            {
                object[] args = { sym, pip };
                method[(int)dm.get_pip_value].Invoke(inst, args);
            }
            return pip[0];
        }

        //локальный путь для базы котировок
        private string storagePath(string path)
        {
            if (method[(int)dm.storagePath] != null)
            {
                char[] cpath = path != null && path.Length > 0 ? path.ToCharArray() : new char[256];
                object[] args = { cpath };
                method[(int)dm.storagePath].Invoke(inst, args);
                return c2a(cpath);
            }
            return "";
        }

        public string StoragePath
        {
            get
            {
                return storagePath(null);
            }
            set
            {
                storagePath(value);
            }
        }

        public void ShowDialog()
        {
            method[(int)dm.ShowDialog].Invoke(inst, null);
        }

        private string GetWorkingDir()
        {
            if (method[(int)dm.GetWorkingDir] != null)
            {
                char[] cpath = new char[256];
                object[] args = { cpath };
                method[(int)dm.GetWorkingDir].Invoke(inst, args);
                return c2a(cpath);
            }
            return "";
        }
        private void SetWorkingDir(string path)
        {
            if (method[(int)dm.SetWorkingDir] != null)
            {
                object[] args = { path };
                method[(int)dm.GetWorkingDir].Invoke(inst, args);
            }
        }
        public string WorkingDir
        {
            get { return GetWorkingDir(); }
            set { SetWorkingDir(value); }
        }
    }
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~//
}

