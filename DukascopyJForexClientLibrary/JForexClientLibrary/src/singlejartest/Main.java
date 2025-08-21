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

import com.dukascopy.api.system.ISystemListener;
import com.dukascopy.api.system.IClient;
import com.dukascopy.api.system.ClientFactory;
//import com.dukascopy.api.IHistory;
import com.dukascopy.api.Instrument;
//import com.dukascopy.api.IContext;

import java.util.HashSet;
import java.util.Set;

import org.slf4j.LoggerFactory;
import org.slf4j.Logger;

import java.io.BufferedReader;
import java.io.InputStreamReader;

//public enum eactn  { ADD_SYM, REM_SYM, HIST, LIST};
/**
 * This small program demonstrates how to initialize Dukascopy client and start a strategy
 */
public class Main {
    private static final Logger LOGGER = LoggerFactory.getLogger(Main.class);

    //url of the DEMO jnlp
    private static String jnlpUrl = "https://www.dukascopy.com/client/demo/jclient/jforex.jnlp";
    //user name
    private static String userName = "DEMO2demo";
    //password
    private static String password = "demo";
    //action
    private static String action = "get_history";
    //symbol
    private static Instrument symbol = Instrument.fromString("USDCHF".substring(0,3)+"/"+"USDCHF".substring(3)); 
    //period
    private static long period = 3600000;
    //length
    private static int length = 50;
    //date of last existing bar
    private static long last_date = 0;
    private static long now_date = 1275172864906L; 
    private static long lhID=-1;
    private static int testOffline = 0;
    
    public static void main(String[] args) throws Exception {

    	if(args.length!=9){
            LOGGER.info("Usage: java -jar JFclient get_history|listen sever-url login password symbol period(in milliseconds) length(bars)|0 date_of_last_resieved_bar|0 current_time|0");
            //actually we are use only listen on startup and all history getting in a cycle at end of this function
            //System.exit(1);
    	}else{
    		LOGGER.info(args[0]);
    		action = args[0];
    		jnlpUrl = args[1];
    		userName = args[2];
    		password = args[3];
    		symbol = Instrument.fromString(args[4].substring(0,3)+"/"+args[4].substring(3));
    		period = Long.parseLong(args[5]);
    		length = Integer.parseInt(args[6]);
    		last_date = Long.parseLong(args[7]);
    		now_date = Long.parseLong(args[8]);
    		LOGGER.info(symbol.name() + " " + period);
        }
   			
    	if(testOffline!=0)
        {
    		System.out.println("&onConnect:");
    		
            InputStreamReader isr = new InputStreamReader(System.in);
            BufferedReader br = new BufferedReader(isr);
    		String stra = br.readLine();
    		System.out.println(stra);
    		
        	long tm = 1275172864906L;
        	double O = 1.2345, H = 1.2355, L = 1.2333, C = 1.2341;
        	int itm = 12751728;
        	for(int i =0; i<length; i++){
            	int delta = (itm % 17)+8;
            	itm+=(itm % 19);
            	tm+=period;
        		System.out.println("&onData: "+tm+ // &onBar: -- real-time fill, &onData: -- fill array
	    			" "+symbol.name()+
	    			" "+period +
	    			" "+(O+delta/10000.0)+
	    			" "+(H+delta/10000.0)+
	    			" "+(L+delta/10000.0)+
	    			" "+(C+delta/10000.0)+
	    			" CUSTOM_TF" +
	    			" BID"
	    			);

        		System.out.println("&onData: "+tm+ // &onBar: -- real-time fill, &onData: -- fill array
	    			" "+symbol.name()+
	    			" "+period +
	    			" "+(O+delta/10000.0+0.0003)+
	    			" "+(H+delta/10000.0+0.0003)+
	    			" "+(L+delta/10000.0+0.0003)+
	    			" "+(C+delta/10000.0+0.0003)+
	    			" CUSTOM_TF" +
	    			" ASK"
	    			);
        	}
        	System.out.println("&onProgress: allDataLoaded bars_loaded: " + 100 + " "+symbol.name()+" "+period);
        	System.out.println("&onProgress: allDataLoaded bars_loaded: " + 100 + " "+symbol.name()+" "+period);
        	
        //	Thread.sleep(3000);

        	for(int i =0; i<1; i++){
        		Thread.sleep(300);
        		long delta = 3;
        		itm-=delta;
        		tm+=delta;
            	System.out.println("&onTick: "+tm+ 
    	    			" "+symbol.name()+
    	    			" "+(O+delta/10000)+
    	    			" "+(H+delta/10000)+" " + "(Time/Symbol/Bid/Ask)"
    	    			);
        	}
        	return;
        }
    	
        //get the instance of the IClient interface
        final IClient client = ClientFactory.getDefaultInstance();


        //set the listener that will receive system events
        client.setSystemListener( 
          new ISystemListener() {
            private int lightReconnects = 3;

        	@Override
        	public void onStart(long processId) {
                LOGGER.info("Strategy started: " + processId);
        	}

			@Override
			public void onStop(long processId) {
                LOGGER.info("Strategy stopped: " + processId);
                if (client.getStartedStrategies().size() == 0) {
                    System.exit(0);
                }
                if(processId == lhID) lhID = -1;
			}

			@Override
			public void onConnect() {
                LOGGER.info("Connected");
                System.out.println("&onConnect:");
                lightReconnects = 3;
			}

			@Override
			public void onDisconnect() {
                LOGGER.warn("Disconnected");
                System.out.println("&onDisconnect:");
                if (lightReconnects > 0) {
                    client.reconnect();
                    --lightReconnects;
                } else {
                    try {
                        //sleep for 10 seconds before attempting to reconnect
                        Thread.sleep(10000);
                    } catch (InterruptedException e) {
                        //ignore
                    }
                    try {
                        client.connect(jnlpUrl, userName, password);
                    } catch (Exception e) {
                        LOGGER.error(e.getMessage(), e);
                    }
                }
			}
		  }
        );

        LOGGER.info("Connecting..."+jnlpUrl+" "+userName+" "+password);
        //connect to the server using jnlp, username and password
        try {
            client.connect(jnlpUrl, userName, password);
        } catch (Exception e) {
            LOGGER.error(e.getMessage(), e);
            System.exit(2);
        }

        //wait for it to connect
        int i = 10; //wait max ten seconds
        while (i > 0 && !client.isConnected()) {
            Thread.sleep(1000);
            i--;
        }
        if (!client.isConnected()) {
            LOGGER.error("Failed to connect Dukascopy servers");
            System.out.println("&onConnectionFailed:");
            if(testOffline!=0)
            	System.exit(1);
        }

        //subscribe to the instruments
        Set<Instrument> instruments = new HashSet<Instrument>();
        //if(action.equals("listen"))
        //{
        	LOGGER.info("Subscribing instruments..." + symbol.name());
        	instruments.add(symbol);
        	client.setSubscribedInstruments(instruments);
        //}
        	
        //start the strategy
        LOGGER.info("Starting data listen");
        client.startStrategy(new JFListener());
        
        //now it's running
        System.out.println("client strategy started");
        
        if(action.equals("get_history"))
        {
	        //start the strategy load_history
	        LOGGER.info("Starting get_history");
	        lhID = client.startStrategy(new JFHistory(symbol, period, length, last_date, now_date));
        }

        try {

            InputStreamReader isr = new InputStreamReader(System.in);
            BufferedReader br = new BufferedReader(isr);
           
	        while(true)
	        {
	        	
	        	String str = br.readLine();
	        	System.out.println("Echo: "+str);
	        	String[] lines = str.split(" ");
	        	if(lines.length >= 2)
	        	{
	        		lines[1] = lines[1].substring(0,3)+"/"+lines[1].substring(3);
	        		symbol = Instrument.fromString(lines[1]);
	        		action = lines[0];
	        		System.out.println("lines.length = "+lines.length+" symstring = "+lines[1]+" actn="+action);
	        		if(symbol != null){
	        			System.out.println(1+ " symbol = "+symbol.name() +" instruments.contains(symbol):"+ instruments.contains(symbol));
			        	if(action.equals("add_symbol") && !instruments.contains(symbol))
			        	{
		        			instruments.add(symbol);
		        			client.setSubscribedInstruments(instruments);
		        	        System.out.println("client setSubscribedInstruments add "+symbol.name());
			        	}
			        	else if (action.equals("remove_symbol") && instruments.contains(symbol))
			        	{
			        		instruments.remove(symbol);
		        			client.setSubscribedInstruments(instruments);
		        	        System.out.println("client setSubscribedInstruments del "+symbol.name());
			        	}
			        	else if (action.equals("get_history") && lines.length == 6)
			        	{
			        		//System.out.println(4);
			        		long period = Long.parseLong(lines[2]);
			        		int len = Integer.parseInt(lines[3]);
			        		long last_date = Long.parseLong(lines[4]);
			        		long now_date = Long.parseLong(lines[5]);
			        		if(lhID >=0)
			        			System.out.println("get history "+ symbol.name()+" "+ period+" "+ len+" "+ last_date+" "+ now_date+ " lhID = "+ lhID);
			        		while(lhID >=0)
			        			Thread.sleep(100);
			        		System.out.println("get history "+ symbol.name()+" "+ period+" "+ len+" "+ last_date+" "+ now_date+ " lhID = "+ lhID);
			                lhID = client.startStrategy(new JFHistory(symbol, period, len, last_date, now_date));
		        		}
        			}
        		}
        	}
        } catch (Exception e) {
            e.printStackTrace();
            System.out.println(6+" Exception caused");
        }
        System.out.println("End of while(System.in.read(bytes)>0) in JFClient.Main");
    }
}
