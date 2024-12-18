/*using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Epic.OnlineServices;
using Epic.OnlineServices.P2P;

namespace PleaseResync
{
    public class EOSSessionAdapter : SessionAdapter
    {
        private readonly ProductUserId localId;
        private readonly ProductUserId[] remoteIds;

        public EOSSessionAdapter(ProductUserId id)
        {
            localId = id;
            remoteIds = new ProductUserId[Session.LIMIT_DEVICE_COUNT];
        }

        public uint SendTo(uint deviceId, DeviceMessage message)
        {
            MemoryStream writerStream = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(writerStream);
            message.Serialize(writer);
            var packet = writerStream.ToArray();
            EOSSDKComponent.Instance.SendPacket(remoteIds[deviceId], packet);
            return 0;
        }

        public List<(uint size, uint deviceId, DeviceMessage message)> ReceiveFrom()
        {
            var messages = new List<(uint size, uint deviceId, DeviceMessage message)>();
            if (remoteIds.Length > 0)
            {
                while(EOSSDKComponent.Instance.PacketsStillQueued())
                {
                    var packet = EOSSDKComponent.Instance.ReceivePacket(out ProductUserId id);
                    if (packet == null) return messages;
                    
                    MemoryStream readerStream = new MemoryStream(packet);
                    BinaryReader reader = new BinaryReader(readerStream);
                    var tempMessage = GetMessageType(reader);
                    messages.Add(((uint)packet.Length, GetIdIndex(id), tempMessage));
                }
            }
            return messages;
        }

        public void AddRemote(uint deviceId, object remoteConfiguration)
        {
            if (remoteConfiguration is ProductUserId remoteId)
            {
                remoteIds[deviceId] = remoteId;
            }
            else
            {
                throw new Exception($"Remote configuration must be of type {typeof(ProductUserId)}");
            }
        }

        public void AddSpectator(uint deviceId, object remoteConfiguration) {}

        public uint GetIdIndex(ProductUserId id)
        {
            for (uint deviceId = 0; deviceId < remoteIds.Length; deviceId++)
            {
                if (id == remoteIds[deviceId])
                {
                    return deviceId;
                }
            }
            throw new Exception($"Device ID not found for endpoint {id.InnerHandle}");
        }

        public void Close() {}

        //Get message type for the new serialization method
        public DeviceMessage GetMessageType(BinaryReader br)
        {
            DeviceMessage finalMessage = null;
            uint ID = br.ReadUInt32();
            switch(ID)
            {
                case 1:
                    finalMessage = new DeviceSyncMessage(br);
                    break;
                case 2:
                    finalMessage = new DeviceSyncConfirmMessage(br);
                    break;
                case 3:
                    finalMessage = new DeviceInputMessage(br);
                    break;
                case 4:
                    finalMessage = new DeviceInputAckMessage(br);
                    break;
                case 5:
                    finalMessage = new HealthCheckMessage(br);
                    break;
            }

            return finalMessage;
        }
    }
}*/
