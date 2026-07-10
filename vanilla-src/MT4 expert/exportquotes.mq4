//+------------------------------------------------------------------+
//|                                                 exportquotes.mq4 |
//|                                                        v. 0.0007 |
//|                                                           isabek |
//|                                                 isabek.110mb.com |
//+------------------------------------------------------------------+
#property copyright "isabek"
#property link      "isabek.110mb.com"

#import "kernel32.dll"
 int CreateFileA(string lpFileName, int dwDesiredAccess, int dwShareMode, int lpSecurityAttributes, int dwCreationDisposition,
                 int dwFlagsAndAttributes, int hTemplateFile);
 bool CloseHandle(int hObject);

#include <stdlib.mqh>
#include <WinUser32.mqh>
#include <stderror.mqh>


extern int historyBars = 50000;
extern int updateBars = 100;
extern string skl_delimiter = "|";


#define SYMLIST 1000
#define WM_MT4INIT 0x0700
#define WM_MT4HISTORY 0x0701
#define WM_MT4UPDATE 0x0702
#define WM_MT4TICK 0x0704
#define WM_MT4TICK_BID 0x0705
#define WM_MT4TICK_ASK 0x0706
#define WM_MT4CONNECTIONLOST 0x0703

bool debug=false;
bool isFirstRun;
int aStream = PERIOD_H1;
int bStream = PERIOD_D1;
string delimiter = ",";
int tickCount, expTime, bigCount;
string symbols[SYMLIST];
string codes[SYMLIST];
datetime lastTime[SYMLIST];
datetime lastTimeB[SYMLIST];
datetime lastTimeTick[SYMLIST];
//double updatesH1[SYMLIST][10][6], updatesD1[SYMLIST][10][6];
int lastBars[SYMLIST], lastBarsB[SYMLIST];
int wDesc = 0;
int hWnd;
string null;
bool disconnected;

int init()
{
  int i;
  
  check4files();
  
  gethWnd();
  SendMsg(WM_MT4INIT, 0, 0);
  
  disconnected = false;

  Print("exp qts init");

  getlist(symbols, lastTime, codes, lastTimeB);

  //if(historyBars > Bars) historyBars = Bars;
  if(debug) bigCount=GetTickCount();

  for(i=0; i<ArraySize(symbols); i++) 
  {
    lastTimeTick[i]=0;
    
    if(debug) tickCount=GetTickCount();
    exportBarsA(true, symbols[i], codes[i], aStream);
    if(debug) expTime=GetTickCount()-tickCount;
    if(debug) Print("Экспорт ", historyBars, " баров ", symbols[i] ," занял ", expTime/1000, " секунд, ", 
                    expTime%1000, " миллисекунд.");
    lastTime[i]=0;

    if(debug) tickCount=GetTickCount();
    exportBarsA(true, symbols[i], codes[i], bStream);
    if(debug) expTime=GetTickCount()-tickCount;
    if(debug) Print("Экспорт ", historyBars, " баров ", symbols[i] ," занял ", expTime/1000, " секунд, ", 
                    expTime%1000, " миллисекунд.");
    lastTimeB[i]=0;
  }

  if(debug) expTime=GetTickCount()-bigCount;
  if(debug) Print("Экспорт всех инструментов занял ", expTime/1000, " секунд, ", expTime%1000, " миллисекунд.");
  isFirstRun = true;
  
  check();

  return(0);
}

int deinit()
{
   return(0);
}

int start()
{
//   if(disconnected)
//   {
//      disconnected = false; //очевидно что уже коннектед
//      check();
//   }
   return(0);
}

int check()
{
//  if(isFirstRun)
//  {
//    isFirstRun = false;
//    return(0);
//  }
  while(true)
  {
      int prevHWnd = hWnd;

      gethWnd();

      RefreshRates();
      
      if(prevHWnd != hWnd)
      {
         getlist(symbols, lastTime, codes, lastTimeB);
         for(int j=0; j<ArraySize(symbols); j++) 
         {
           lastTimeTick[j]=0;
           exportBarsA(false, symbols[j], codes[j], aStream);
           exportBarsA(false, symbols[j], codes[j], bStream);
         }
      }

//      if(IsConnected()==false){
//        SendMsg(WM_MT4CONNECTIONLOST, 0, 0);
//        disconnected = true;
//        return(0);
//      }
      getnewlist(symbols, lastTime, codes, lastTimeB);

      if(debug) bigCount=GetTickCount();
      bool isexport = false;
      datetime datetemp;
      for(int i=0; i<ArraySize(symbols); i++) 
      {
       //send ticks realtime
       if(MarketInfo(symbols[i], MODE_TIME) > lastTimeTick[i] || lastTimeTick[i] == 0)
       {
   
         lastTimeTick[i] =  MarketInfo(symbols[i], MODE_TIME);
   
         //проверим является ли инструмент валютной парой, и если да, то подкорректируем точность цены до 2-х/4-х знаков.
         int digits = MarketInfo(symbols[i], MODE_DIGITS);
         double pip = MarketInfo(symbols[i], MODE_POINT);
         if(StringLen(symbols[i]) == 6 && (digits == 3 || digits == 5)){
            bool superdigit = true;
            for(int c=0; c<6; c++){
               int a = StringGetChar(symbols[i], c);
               if(a < 'A' || a > 'Z'){
                  superdigit = false;
                  break;
               }
            }
            if(superdigit)
            {
               digits--;
               pip*=10;
            }
         }
         //преобразование цены в целое число(фикседпоинт:) с одновременной обрезкой лишних знаков.
         int bid = (MarketInfo(symbols[i], MODE_BID)/pip);
         int ask = (MarketInfo(symbols[i], MODE_ASK)/pip);
            //такая себе упаковка: параметры
            // WM_MESSADGE -- старшие 12бит=код символа, 4бита=digits, младшие 16бит=код сообщения
            // WParam -- дата+время в си стиле
            // LParam -- старшие 8 бит=спред младшие 24=бид.
         // SendMsg((StrToInteger(codes[i])<<20)|(digits<<16)|WM_MT4TICK, lastTimeTick[i], ((bid - ask)<<24)|bid);
            // но не работает(
         //======================================================================//
            //и работающая упаковка (передача тика в два сообщения), параметры:
            // WM_MESSADGE -- старшие 16бит=0000 ибо иначе сообщения не проходят, 4бита=digits, младшие 12бит=код сообщения
            // WParam -- дата+время в си стиле
            // LParam -- старшие 12 бит=код символа, младшие 20=бид/аск. 
         SendMsg(((digits+7)<<12)|WM_MT4TICK_BID, lastTimeTick[i], bid|(StrToInteger(codes[i])<<20));
         SendMsg(((digits+7)<<12)|WM_MT4TICK_ASK, lastTimeTick[i], ask|(StrToInteger(codes[i])<<20));
       }
       //--
       if(iTime(symbols[i], aStream, 0) > lastTime[i] || iBars(symbols[i], aStream) > lastBars[i])
       {
         lastTime[i] = iTime(symbols[i], aStream, 0);
         lastBars[i] = iBars(symbols[i], aStream);
         isexport=true;
         if(updateBars > Bars) updateBars = Bars; // что, по идее, маловероятно, но Нассим Талеб :) 
         if(debug) tickCount=GetTickCount();
         exportBarsA(false, symbols[i], codes[i], aStream);
         if(debug) expTime=GetTickCount()-tickCount;
         if(debug) Print("Экспорт ", historyBars, " баров ", symbols[i] ," занял ", expTime/1000, " секунд, ", 
                         expTime%1000, " миллисекунд.");
       }

       if(iTime(symbols[i], bStream, 0) > lastTimeB[i] || iBars(symbols[i], bStream) > lastBarsB[i])
       {
         lastTimeB[i] = iTime(symbols[i], bStream, 0);
         lastBarsB[i] = iBars(symbols[i], bStream);
         isexport=true;
         if(updateBars > Bars) updateBars = Bars; // что, по идее, маловероятно, но Нассим Талеб :) 
         if(debug) tickCount=GetTickCount();
         exportBarsA(false, symbols[i], codes[i], bStream);
         if(debug) expTime=GetTickCount()-tickCount;
         if(debug) Print("Экспорт ", historyBars, " баров ", symbols[i] ," занял ", expTime/1000, " секунд, ", 
                         expTime%1000, " миллисекунд.");
       }         

      }

      if(debug&&isexport) expTime=GetTickCount()-bigCount;
      if(debug&&isexport) Print("Экспорт всех инструментов занял ", expTime/1000, " секунд, ", expTime%1000, " миллисекунд.");

      Sleep(100);
  }
  return(0);
}

bool exportBarsA(bool history, string symbol, string code, int stream)
{
  string file_delimiter = "'";
  string exportFile = symbol + file_delimiter + stream;
  if(history==true) exportFile = exportFile + file_delimiter +"h";
  if(history==false) exportFile = exportFile + file_delimiter + "u";
  exportFile = exportFile + file_delimiter + code;
  prepareBars(symbol, stream);//если еще не загружена история по инструменту
  
  int handle = FileOpen(exportFile, FILE_CSV | FILE_WRITE, delimiter);
  if (handle == -1) AlertFileError(exportFile);
  else
  {
    if(history==true)
    {
      mainExport(handle, historyBars, 0, symbol, stream);
      FileClose(handle);
      SendMsg(WM_MT4HISTORY, code, stream);
    }
    if(history==false)
    {
      mainExport(handle, updateBars, 0, symbol, stream);
      FileClose(handle);
      SendMsg(WM_MT4UPDATE, code, stream);
    }
  }
  
  return(true);
}

void mainExport(int handle, int fromBars, int toBars, string symbol, int stream)
{
  string dateString, timeString;
  for(int i = fromBars-1; i >= toBars; i--)
  {
    dateString = TimeToStr(iTime(symbol, stream, i), TIME_DATE);
    timeString = TimeToStr(iTime(symbol, stream, i), TIME_MINUTES);
    if(dateString=="1970.01.01")
    {
      continue;
    }
    FileWrite(handle, dateString, timeString, iOpen(symbol, stream, i), 
              iHigh(symbol, stream, i), iLow(symbol, stream, i), 
              iClose(symbol, stream, i));
  }
}

void AlertFileError(string fileName)
{
  int fileError = GetLastError();
  //Alert("Ошибка доступа к файлу [", fileName, "].\n", "Код ошибки: ", fileError, " (", ErrorDescription(fileError), ")");
}

void getlist(string &symbolList[], datetime &timeList[], string &codesList[], datetime &timeListB[])
{
  string fileName = "toget.skl";
  string symbol;
  string code;
  string _symbols[SYMLIST];
  string _codes[SYMLIST];
  int _pointer=0;
  int i;
  string temp;

  ArrayResize(_symbols, SYMLIST);
  for(i=0; i<SYMLIST; i++)
  {
    _symbols[i] = "";
  }
  
  int handle = FileOpen(fileName, FILE_CSV|FILE_READ, skl_delimiter);
  if(handle == -1) AlertFileError(fileName);

  while(FileIsEnding(handle)==false)
  {
    symbol = FileReadString(handle);
    code = FileReadString(handle);
    if(FileIsEnding(handle)==true) break;
    if(StringLen(symbol)<2) continue;
    if(SearchString(_symbols, symbol)==-1)
    {
      _symbols[_pointer] = symbol;
      _codes[_pointer] = code;
      _pointer++;
      if(_pointer>=ArraySize(_symbols)) ArrayResize(_symbols, ArraySize(_symbols)+SYMLIST);
    }
  }
  FileClose(handle);

  ArrayResize(_symbols, _pointer);
  ArrayResize(symbolList, ArraySize(_symbols));
  ArrayResize(timeList, ArraySize(_symbols));

  for(i=0; i<ArraySize(_symbols); i++)
  {
    symbolList[i] = _symbols[i];
    codesList[i] = _codes[i];
    timeList[i] = 0;
    timeListB[i] = 0;
  }
}

void prepareBars(string symbol, int stream)
{
  double array1[][6];
  int count=0, len=0;
  while (len<1000)
  {
    len = ArrayCopyRates(array1, symbol, stream);
    Sleep(100);
    count++;
    if(count>5) break;
  }
  
  while(GetLastError() == ERR_HISTORY_WILL_UPDATED)
  {
    Print(symbol + stream + ": last_error == ERR_HISTORY_WILL_UPDATED");
    Sleep(300);
    if(count > 30) break;
    count++;
    len = ArrayCopyRates(array1, symbol, stream);
  }
  Print(symbol + stream + ": Is updated history counter:"+count+" len:"+len);
}

void getnewlist(string &symbolList[], datetime &timeList[], string &codesList[], datetime &timeListB[])
{
  string newsymbols[SYMLIST];
  datetime newtimes[SYMLIST];
  string newcodes[SYMLIST];
  datetime newtimesB[SYMLIST];
  int i, j, k;
  bool todel, toadd;

  getlist(newsymbols, newtimes, newcodes, newtimesB);
  if(ArraySize(newsymbols)<1) return;
  
  for(i=0; i<ArraySize(symbolList); i++)
  {
    todel = true;
    for(j=0; j<ArraySize(newsymbols); j++)
    {
      if(symbolList[i]==newsymbols[j])
      {
        todel=false;
        break;
      }
    }
    if(todel==true)
    {
      if(debug) Print("Symbol removed - ", symbolList[i]);
      DeleteString(symbolList, i);
      DeleteString(codesList, i);
      DeleteDT(timeList, i);
      DeleteDT(timeListB, i);
    }
  }

  for(i=0; i<ArraySize(newsymbols); i++)
  {
    toadd = true;
    for(j=0; j<ArraySize(symbolList); j++)
    {
      if(newsymbols[i]==symbolList[j])
      {
        toadd = false;
        break;
      }
    }
    if(toadd==true)
    {
      if(debug) Print("Symbol added - ", newsymbols[i]);
      if(debug) Print("Symbol added[code] - ", newcodes[i]);
      ArrayResize(symbolList, ArraySize(symbolList)+1);
      symbolList[ArraySize(symbolList)-1]=newsymbols[i];
      ArrayResize(codesList, ArraySize(symbolList));
      codesList[ArraySize(codesList)-1]=newcodes[i];
      ArrayResize(timeList, ArraySize(timeList)+1);
      timeList[ArraySize(timeList)-1]=0;
      ArrayResize(timeListB, ArraySize(timeList));
      timeListB[ArraySize(timeListB)-1]=0;
      
      exportBarsA(true, newsymbols[i], newcodes[i], aStream);
      exportBarsA(true, newsymbols[i], newcodes[i], bStream);
    }
  }
  
}

int DeleteDT(datetime& dt[], int i)
{
  int j, k=ArraySize(dt);

  if (i>=0 && i<k)
  {
    for (j=i; j<k; j++) dt[j]=dt[j+1];
    k=ArrayResize(dt, k-1);
    return(k);
  }
  return(-1);
}

int DeleteString(string& str[], int i)
{
  int j, k=ArraySize(str);

  if (i>=0 && i<k)
  {
    for (j=i; j<k; j++) str[j]=str[j+1];
    k=ArrayResize(str, k-1);
    return(k);
  }
  return(-1);
}

int SearchString(string& strs[], string f) {
  for (int i=0; i<ArraySize(strs); i++) {
    //Print(s[i], " | ", f);
    if (strs[i]==f) return(i);
  }
  return(-1);
}

bool SendMsg(int MsgID, string code, int per)
{
	PostMessageA(hWnd, MsgID, StrToInteger(code), per);
}

int gethWnd()
{
  string fileName = "hWnd.txt";
  string szhWnd = "";
  
  int handle = FileOpen(fileName, FILE_CSV|FILE_READ, skl_delimiter);
  if(handle == -1) AlertFileError(fileName);
  else
  {
    szhWnd = FileReadString(handle);
    FileClose(handle);
  }
  if(szhWnd != "") hWnd = StrToInteger(szhWnd);
  else hWnd = -1;
  return(hWnd);
}

void check4files()
{
  string filename, temp;
  int handle;
  int error;
  
  filename = "hWnd.txt";
  handle = FileOpen(filename, FILE_CSV|FILE_READ);
  if(handle == -1) {
    createfile(filename, "0");
  }

  filename = "toget.skl";
  handle = FileOpen(filename, FILE_CSV|FILE_READ);
  if(handle == -1) {
    temp = Symbol()+"|1";
    createfile(filename, temp);
  }
  
}

void createfile(string filename, string text)
{
  int handle;
  string name = TerminalPath() + "\\experts\\files";
  name = name + "\\";
  name = name + filename;
  //Print(name);
  handle = CreateFileA(name, 0x40000000, 0x0002, NULL, 2, 0x80, NULL);
  CloseHandle(handle);
  
  handle = FileOpen(filename, FILE_CSV|FILE_WRITE);
  if(handle != -1) {
    FileWrite(handle, text);
    FileClose(handle);
  }
}

