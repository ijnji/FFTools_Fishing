using System;
using System.Diagnostics; 
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Text;

namespace FFTools {
    public class Fishing {
        // Program settings.
        private const int DELAY_UPDATE = 200;
        private const int DELAY_REELING = 8000;
        private const int TIMEO_FISHING = 25000;
        private const Keys K_CAST = Keys.D2;
        private const Keys K_HOOK = Keys.D3;
        private const Keys K_MOOCH = Keys.D4;

        // Program fields.
        private static MemoryManager TheMemory = new MemoryManager();
        private enum States {IDLE, FISHING, REELING};
        private static States CurrentState = States.IDLE;
        private static int Timer = 0;

        public static void Main() {
            if (TheMemory.initialize() > 0) Environment.Exit(1);

            while (true) {
                update();
                Thread.Sleep(DELAY_UPDATE);
                Timer = Timer + DELAY_UPDATE;
            }
        }

        private static void update() {
            switch (CurrentState) {
                case States.IDLE :
                    Thread.Sleep(1000);
                    System.Console.WriteLine("IDLE: Casting fishing line.");
                    TheMemory.sendKeyPressMsg(K_CAST, 500);
                    CurrentState = States.FISHING;
                    Timer = 0;
                break;
                case States.FISHING :
                    if ( TheMemory.readFishBite() ) {
                        System.Console.WriteLine("FISHING: Fish bite! Reeling.");
                        TheMemory.sendKeyPressMsg(K_HOOK, 500);
                        CurrentState = States.REELING;
                        Timer = 0;
                        break;
                    } else if ( Timer >= TIMEO_FISHING ) {
                        System.Console.WriteLine("FISHING: " + (Timer / 1000) + " seconds elapsed.");
                    } else if ( (Timer % 5000) == 0 ) {
                        System.Console.WriteLine("FISHING: Fishing timed out. Recasting.");
                        Thread.Sleep(1000);
                        TheMemory.sendKeyPressMsg(K_CAST, 500);
                        Timer = 0;
                        break;
                    }
                break;
                case States.REELING :
                    Thread.Sleep(DELAY_REELING);
                    System.Console.WriteLine("REELING: Casting fishing line."); 
                    TheMemory.sendKeyPressMsg(K_CAST, 500);
                    CurrentState = States.FISHING;
                    Timer = 0;
                break;
            }
        }
    } 
}