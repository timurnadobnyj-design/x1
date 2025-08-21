/*
 * Copyright (c) 2009 Dukascopy (Suisse) SA. All Rights Reserved.
 * 
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 * 
 * -Redistribution of source code must retain the above copyright notice, this
 *  list of conditions and the following disclaimer.
 * 
 * -Redistribution in binary form must reproduce the above copyright notice, 
 *  this list of conditions and the following disclaimer in the documentation
 *  and/or other materials provided with the distribution.
 * 
 * Neither the name of Dukascopy (Suisse) SA or the names of contributors may 
 * be used to endorse or promote products derived from this software without 
 * specific prior written permission.
 * 
 * This software is provided "AS IS," without a warranty of any kind. ALL 
 * EXPRESS OR IMPLIED CONDITIONS, REPRESENTATIONS AND WARRANTIES, INCLUDING
 * ANY IMPLIED WARRANTY OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE
 * OR NON-INFRINGEMENT, ARE HEREBY EXCLUDED. DUKASCOPY (SUISSE) SA ("DUKASCOPY")
 * AND ITS LICENSORS SHALL NOT BE LIABLE FOR ANY DAMAGES SUFFERED BY LICENSEE
 * AS A RESULT OF USING, MODIFYING OR DISTRIBUTING THIS SOFTWARE OR ITS
 * DERIVATIVES. IN NO EVENT WILL DUKASCOPY OR ITS LICENSORS BE LIABLE FOR ANY LOST 
 * REVENUE, PROFIT OR DATA, OR FOR DIRECT, INDIRECT, SPECIAL, CONSEQUENTIAL, 
 * INCIDENTAL OR PUNITIVE DAMAGES, HOWEVER CAUSED AND REGARDLESS OF THE THEORY 
 * OF LIABILITY, ARISING OUT OF THE USE OF OR INABILITY TO USE THIS SOFTWARE, 
 * EVEN IF DUKASCOPY HAS BEEN ADVISED OF THE POSSIBILITY OF SUCH DAMAGES.
 */
package singlejartest;

//import java.sql.Date;

import com.dukascopy.api.*;

public class JFHistory implements IStrategy {
    private IConsole console;
    private IHistory history;
    private IContext icontext;
    Instrument symbol;
    long period;
    int len;
    long last_date, now_date;
    int numberOfBarsLoaded;
    int testOffline = 0;
    private IBar bidBar, askBar;
    boolean symOpened = false;
    
    public JFHistory(Instrument symbol, long period, int len, long last_date, long now_date)
    {
    	this.symbol = symbol;
    	this.period = period;
    	this.len = len;
    	this.last_date = last_date;
    	this.now_date = now_date;
    }

    public void onStart(IContext context) throws JFException {
        this.console = context.getConsole();
        this.history = context.getHistory();
        icontext = context;
        console.getOut().println("Started " + symbol.name()+ " "+len+ " "+last_date);
        
        if(testOffline!=0)
        {
        	long tm = 1275172864906L;
        	double O = 1.2345, H = 1.2355, L = 1.2333, C = 1.2341;
        	int itm = (int)tm;
        	for(int i =0; i<100; i++){
            	int delta = itm % 9-5;
            	itm-=delta;
            	tm+=period;
        		console.getOut().println("&onData: "+tm+ // &onBar: -- real-time fill, &onData: -- fill array
	    			" "+symbol.name()+
	    			" "+period +
	    			" "+(O+delta/10000)+
	    			" "+(H+delta/10000)+
	    			" "+(L+delta/10000)+
	    			" "+(C+delta/10000)+
	    			" CUSTOM_TF" +
	    			" BID"
	    			);

        		console.getOut().println("&onData: "+tm+ // &onBar: -- real-time fill, &onData: -- fill array
	    			" "+symbol.name()+
	    			" "+period +
	    			" "+(O+delta/10000+0.0003)+
	    			" "+(H+delta/10000+0.0003)+
	    			" "+(L+delta/10000+0.0003)+
	    			" "+(C+delta/10000+0.0003)+
	    			" CUSTOM_TF" +
	    			" ASK"
	    			);
        	}
        	console.getOut().println("&onProgress: allDataLoaded bars_loaded: " + 100);
        	console.getOut().println("&onProgress: allDataLoaded bars_loaded: " + 100);
        }else{
        	readHistory(OfferSide.ASK);
        	readHistory(OfferSide.BID);
        }
        if(symOpened){
        	//;
       
        console.getOut().println("history.getBar " + long2period(period).name());
        //if(len == 1 && last_date == 0 && now_date == 0)
        //{
        	askBar = history.getBar(symbol, long2period(period), OfferSide.ASK, 0);
        	bidBar = history.getBar(symbol, long2period(period), OfferSide.BID, 0);
        	
        	console.getOut().println(
        			"&currentBar: "+askBar.getTime()+
        			" "+symbol.name()+
        			" "+period +
        			" "+((bidBar.getOpen()+askBar.getOpen())/2) +
        			" "+askBar.getHigh()+
        			" "+bidBar.getLow()+
        			" "+((bidBar.getClose()+askBar.getClose())/2)+
        			" "+long2period(period).name()
        			);
        //}
        //console.getOut().println("&onProgress: allDataLoaded bars_loaded: " + numberOfBarsLoaded);
        //	context.stop();
        }
    }


    public void onStop() throws JFException {
        console.getOut().println("JFHistory::onStop");
    }

    public void onTick(Instrument instrument, ITick tick) throws JFException
    {
    /*	if(instrument.name().equals(symbol.name()))	{
    		
        console.getOut().println("history.getBar " + long2period(period).name());
        //if(len == 1 && last_date == 0 && now_date == 0)
        //{
        	askBar = history.getBar(symbol, long2period(period), OfferSide.ASK, 0);
        	bidBar = history.getBar(symbol, long2period(period), OfferSide.BID, 0);
        	
        	console.getOut().println(
        			"&currentBar: "+askBar.getTime()+
        			" "+symbol.name()+
        			" "+period +
        			" "+((bidBar.getOpen()+askBar.getOpen())/2) +
        			" "+askBar.getHigh()+
        			" "+bidBar.getLow()+
        			" "+((bidBar.getClose()+askBar.getClose())/2)+
        			" "+long2period(period).name()
        			);

        	symOpened = true;
    	}*/
    }
    
    public void onBar(Instrument instrument, Period period, IBar askBar, IBar bidBar){
    }

    public void onMessage(IMessage message) throws JFException {
    }

    public void onAccount(IAccount account) throws JFException {
    }

    public Period long2period(long interval)
    {
        long m60  = 60 * 60 * 1000,
        m240 = 240 * 60 * 1000,
        day  = 24 * 60 * 60 * 1000,
        week = 7 * 24 * 60 * 60 * 1000,
        month= 30 * 24 * 60 * 60 * 1000,
        year = 365240 * 24 * 60 * 60;
        
        if(interval == m60) return Period.ONE_HOUR; 
        if(interval == m240) return Period.FOUR_HOURS; 
        if(interval == day) return Period.DAILY; 
        if(interval == week) return Period.WEEKLY; 
        if(interval == month) return Period.MONTHLY; 
        if(interval == year) return Period.ONE_YEAR;
    	return Period.ONE_HOUR;
    }

    public void readHistory(OfferSide bid_ask_side) throws JFException {
        numberOfBarsLoaded=0;
        Period tf = long2period(period);
        long from, to;// = history.getStartTimeOfCurrentBar(symbol, tf);
        //console.getOut().println("history.getStartTimeOfCurrentBar("+symbol.name()+":"+tf+" returns "+to);
        //if(to==-1)
        	to = history.getBarStart(tf, now_date /* + period*/);
        
        
        
        if(last_date != 0) from = history.getBarStart(tf, last_date);
        else if (len > 0)  from = history.getBarStart(tf, now_date - period * len);
        else from = history.getBarStart(tf, now_date - period * 100);
        
        history.readBars(symbol, tf, bid_ask_side, from, to,
        	new LoadingDataListener() {
        	    public void newTick(Instrument instrument, long time, double ask, double bid, double askVol, double bidVol) {
        	        //no ticks expected, because we are loading bars
        	    }
        	    public void newBar(Instrument instrument, Period period, OfferSide side, long time, double open, double close, double low, double high, double vol) {
        	    	//if(open==high && open==low && open==close)return;
        	        ++numberOfBarsLoaded;
        	    	console.getOut().println(
        	    			"&onData: "+time+ // &onBar: -- real-time fill, &onData: -- fill array
        	    			" "+instrument.name()+
        	    			" "+period.getInterval() +
        	    			" "+open+
        	    			" "+high+
        	    			" "+low+
        	    			" "+close+
        	    			" "+period.name()+
        	    			" "+side.name()
        	    			);
        	    }
        	},
        	new LoadingProgressListener() {
        	    public void dataLoaded(long startTime, long endTime, long currentTime, String information) {
        	    	console.getOut().println("&onProgress: " + information);
        	    }
        	    public void loadingFinished(boolean allDataLoaded, long startTime, long endTime, long currentTime) {
	        		if (allDataLoaded) {
	        	        console.getOut().println("&onProgress: allDataLoaded bars_loaded: " + numberOfBarsLoaded +
	        	            		" " + symbol.name() + " " + period + (last_date == 0 ? " sync" : " async"));
	        		  //  icontext.stop();
	        		} else {
	        		    console.getOut().println("&onProgress: For some reason loading failed or was canceled by the user");
	        		    icontext.stop();
	        		}
        	    }
        	    public boolean stopJob() {
	        		return false;
	        	}
	        }
        );
    }
}