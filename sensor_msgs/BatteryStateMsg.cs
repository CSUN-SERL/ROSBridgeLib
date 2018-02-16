using System;
using System.Runtime.CompilerServices;
using SimpleJSON;
using ROSBridgeLib.std_msgs;

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
    public class BatteryStateMsg : Core.ROSBridgeMessage
    {
        public HeaderMsg Header { get; private set; }
        public float Voltage { get; private set; } // [V]
        public float Current { get; private set; } // [A]
        public float Charge { get; private set; } // 0..1
        public float Capacity { get; private set; }
        public float DesignCapacity { get; private set; }
        public float Percentage { get; private set; }
        public PowerSupplyStatus PowerSupplyStatus { get; private set; }
        public PowerSupplyHealth PowerSupplyHealth { get; private set; }
        public PowerSupplyTechnology PowerSupplyTechnology { get; private set; }
        public bool Present { get; private set; }
        public float[] CellVoltage { get; private set; }
        public string Location { get; private set; }
        public string SerialNumber { get; private set; }

        public override string ROSMessageType
        {
            get { return "sensor_msgs/BatteryState"; }
        }

        public BatteryStateMsg(){ }
        
        public BatteryStateMsg(JSONNode msg)
        {
            InternalDeserialize(msg);
        }
        
        public override void Deserialize(JSONNode msg)
        {
            InternalDeserialize(msg);
        }

        private void InternalDeserialize(JSONNode msg)
        {
            Header = new HeaderMsg(msg["header"]);

            float f;
            Voltage = float.TryParse(msg["voltage"], out f) ? f : 0;
            Current = float.TryParse(msg["current"], out f) ? f : 0;
            Charge = float.TryParse(msg["charge"], out f) ? f : 0;
            Capacity = float.TryParse(msg["capacity"], out f) ? f : 0;
            DesignCapacity = float.TryParse(msg["design_capacity"], out f) ? f : 0;
            Percentage = float.TryParse(msg["percentage"], out f) ? f : 0;
            
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

            bool present;
            Present = bool.TryParse(msg["present"], out present) ? present : false;

            JSONNode voltageJson = msg["cell_voltage"];
            int count = voltageJson.Count;
            CellVoltage = new float[count];
            for (int i = 0; i < count; ++i)
            {
                float voltage;
                CellVoltage[i] = float.TryParse(voltageJson[i], out voltage) ? voltage : 0;
            }

            Location = msg["location"];
            SerialNumber = msg["serial_number"];
        }
        
        public override string ToString()
        {
            return $"{ROSMessageType} [header={Header}, voltage={Voltage}, current={Current}, charge={Charge}, capacity={Capacity}, design_capacity={DesignCapacity}, percentage={Percentage}, power_supply_status={(int)PowerSupplyStatus}, power_supply_health={(int)PowerSupplyHealth}, power_supply_technology={(int)PowerSupplyTechnology}, present={Present}, cell_voltage={CellVoltageToString()}, location={Location}, serial_number={SerialNumber}]";                                                                                               
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