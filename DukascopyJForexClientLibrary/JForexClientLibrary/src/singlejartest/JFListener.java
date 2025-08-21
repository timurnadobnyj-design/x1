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

import com.dukascopy.api.*;

public class JFListener implements IStrategy {
	
    private IConsole console;
    
    public void onStart(IContext context) throws JFException {
        this.console = context.getConsole();
        console.getOut().println("Started");     
    }

    public void onStop() throws JFException {
        console.getOut().println("Stopped");
    }

    public void onTick(Instrument instrument, ITick tick) throws JFException 
    {
    	console.getOut().println("&onTick: "+tick.getTime()+" "+instrument.name()+" " + tick.getBid()+" " + tick.getAsk()+" " + "(Time/Symbol/Bid/Ask)");
    }

    public void onBar(Instrument instrument, Period period, IBar askBar, IBar bidBar)
    {
    	console.getOut().println(
    			"&onBar: "+askBar.getTime()+
    			" "+instrument.name()+
    			" "+period.getInterval() +
    			" "+((bidBar.getOpen()+askBar.getOpen())/2) +
    			" "+askBar.getHigh()+
    			" "+bidBar.getLow()+
    			" "+((bidBar.getClose()+askBar.getClose())/2)+
    			" "+period.name()
    			);
/*    	console.getOut().println(
    			"&onBar: "+askBar.getTime()+
    			" "+instrument.name()+
    			" "+period.getInterval() +
    			" "+bidBar.getOpen()+
    			" "+bidBar.getHigh()+
    			" "+bidBar.getLow()+
    			" "+bidBar.getClose()+
    			" "+period.name()
    			);*/
    }

    public void onMessage(IMessage message) throws JFException {
    }

    public void onAccount(IAccount account) throws JFException {
    }
}