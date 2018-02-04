using System.Runtime.InteropServices;
using SimpleJSON;
using ROSBridgeLib.std_msgs;
using UnityEngine;

namespace ROSBridgeLib.sensor_msgs
{
    public enum PowerSupplyStatus
    {
        Unknown = 0,
        Charging = 1,
        Discharging = 2,
        NotCharging = 3,
        Full = 4
    }
        
    public enum PowerSupplyHealth
    {
        Unknown = 0,
        Good = 1,
        OverHeat = 2,
        Dead = 3,
        OverVoltage = 4,
        UnspecFailure = 5,
        Cold = 6,
        WatchdogTimerExpire = 7,
        SafetyTimerExpire = 8
    }

    public enum PowerSupplyTechnology
    {
        Unknown = 0,
        NIMH = 1,
        LION = 2,
        LIPO = 3,
        LIFE = 4,
        NICD = 5,
        LIMN = 6
    }
    
    /// <summary>
    /// Represent battery status from SYSTEM_STATUS
    /// </summary>
    public class BatteryStateMsg : ROSBridgeMsg
    {
        public HeaderMsg Header;
        public float Voltage; // [V]
        public float Current; // [A]
        public float Charge; // 0..1
        public float Capacity;
        public float DesignCapacity;
        public float Percentage;
        public PowerSupplyStatus PowerSupplyStatus;
        public PowerSupplyHealth PowerSupplyHealth;
        public PowerSupplyTechnology PowerSupplyTechnology;
        public bool Present;
        public float[] CellVoltage;
        public string Location;
        public string SerialNumber;

        public BatteryStateMsg(JSONNode msg)
        {
            Header = new HeaderMsg(msg["header"]);
            float.TryParse(msg["voltage"], out Voltage);
            float.TryParse(msg["current"], out Current);
            float.TryParse(msg["charge"], out Charge);
            float.TryParse(msg["capacity"], out Capacity);
            float.TryParse(msg["design_capacity"], out DesignCapacity);
            float.TryParse(msg["percentage"], out Percentage);

            int pss;
            if(int.TryParse(msg["power_supply_status"], out pss))
            {
                PowerSupplyStatus = (PowerSupplyStatus) pss;
            }

            int psh;
            if (int.TryParse(msg["power_supply_health"], out psh))
            {
                PowerSupplyHealth = (PowerSupplyHealth) psh;
            }

            int pst;
            if (int.TryParse(msg["power_supply_technology"], out pst))
            {
                PowerSupplyTechnology = (PowerSupplyTechnology) pst;
            }

            bool.TryParse(msg["present"], out Present);

            JSONNode voltageJson = msg["cell_voltage"];
            int count = voltageJson.Count;
            CellVoltage = new float[count];
            for (int i = 0; i < count; ++i)
            {
                float voltage;
                bool parsed = float.TryParse(voltageJson[i], out voltage);
                CellVoltage[i] = parsed ? voltage : 0;
            }

            Location = msg["location"];
            SerialNumber = msg["serial_number"];
        }
        
        public static string GetMessageType()
        {
            return "sensor_msgs/BatteryState";
        }

        public override string ToString()
        {
            return $"{GetMessageType()} [header={Header}, voltage={Voltage}, current={Current}, charge={Charge}, capacity={Capacity}, design_capacity={DesignCapacity}, percentage={Percentage}, power_supply_status={(int)PowerSupplyStatus}, power_supply_health={(int)PowerSupplyHealth}, power_supply_technology={(int)PowerSupplyTechnology}, present={Present}, cell_voltage={CellVoltageToString()}, location={Location}, serial_number={SerialNumber}]";                                                                                               
        }

        public override string ToYAMLString()
        {
            return $"{{\"header\" : {Header}, \"voltage\" : {Voltage}, \"current\" : {Current}, \"charge\" : {Charge}, \"capacity\" : {Capacity}, \"design_capacity\" : {DesignCapacity}, \"percentage\" : {Percentage}, \"power_supply_status\" : {(int)PowerSupplyStatus}, \"power_supply_health\" : {(int)PowerSupplyHealth}, \"power_supply_technology\" : {(int)PowerSupplyTechnology}, \"present\" : {Present}, \"cell_voltage\" : {CellVoltageToString()}, \"location\" : {Location}, \"serial_number\" : {SerialNumber}}}";                                                                                                                                                                                   
                 }
        
        private string CellVoltageToString()
        {
            int last = CellVoltage.Length - 1;
            
            string array = "[";
            
            for (int i = 0; i < last; i++) 
            {
                array += CellVoltage[i] + ",";
            }
            
            array += CellVoltage[last] + "]";

            return array;
        }
    }
}