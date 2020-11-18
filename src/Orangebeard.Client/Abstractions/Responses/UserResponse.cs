﻿using Orangebeard.Client.Abstractions.Models;
using Orangebeard.Client.Converters;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Orangebeard.Client.Abstractions.Responses
{
    [DataContract]
    public class UserResponse
    {
        [DataMember(Name = "fullName")]
        public string Fullname { get; set; }

        [DataMember(Name = "email")]
        public string Email { get; set; }

        [DataMember(Name = "assignedProjects")]
        public IDictionary<string, ProjectAssigment> AssignedProjects { get; set; }
    }

    [DataContract]
    public class ProjectAssigment
    {
        [DataMember(Name = "projectRole")]
        private string ProjectRoleString { get; set; }

        public ProjectRole ProjectRole => EnumConverter.ConvertTo<ProjectRole>(ProjectRoleString);
    }
}
