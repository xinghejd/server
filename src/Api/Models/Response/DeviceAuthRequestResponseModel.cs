﻿using Bit.Core.Auth.Utilities;
using Bit.Core.Entities;
using Bit.Core.Enums;
using Bit.Core.Models.Api;

namespace Bit.Api.Models.Response;

public class DeviceAuthRequestResponseModel : ResponseModel
{
    public DeviceAuthRequestResponseModel(Device device)
        : base("device")
    {
        if (device == null)
        {
            throw new ArgumentNullException(nameof(device));
        }

        Id = device.Id;
        Name = device.Name;
        Type = device.Type;
        Identifier = device.Identifier;
        CreationDate = device.CreationDate;
        IsTrusted = device.IsTrusted();
        DevicePendingAuthRequest = new PendingAuthRequest();
    }

    public Guid Id { get; set; }
    public string Name { get; set; }
    public DeviceType Type { get; set; }
    public string Identifier { get; set; }
    public DateTime CreationDate { get; set; }
    public bool IsTrusted { get; set; }

    public PendingAuthRequest DevicePendingAuthRequest { get; set; }

    public class PendingAuthRequest
    {
        public Guid Id { get; set; }
        public DateTime CreationDate { get; set; }
    }
}
