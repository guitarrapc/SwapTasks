[![Build status](https://ci.appveyor.com/api/projects/status/lvqx1aaipttqtcaq?svg=true)](https://ci.appveyor.com/project/guitarrapc/swaptasks)
[![NuGet status](https://img.shields.io/nuget/v/SwapTasks.svg)](https://www.nuget.org/packages/SwapTasks)

## SwapTasks

MSBuild Custom Tasks for .NETCore

## Install

You can install by [NuGet](https://www.nuget.org/packages/SwapTasks).

```powershell
PM> Install-Package SwapTasks
```

## Supported Platform

- .NETStandard 2.0
- .NET Framework (FullClr)

## Current Tasks

### SwapFile

#### Description

Swap file with source and destination.

#### Example

```xml
  <Target Name="SwapTask" BeforeTargets="Build">
    <SwapFile Trigger="$(Trigger)" Fallback="Development" Configuration="$(Configuration)" FileName="app" Extension="config" SourceDir="$(MSBuildThisFileDirectory)" DestinationDir="$(MSBuildThisFileDirectory)" />
  </Target>
```

#### Equivalent

```xml
    <Target Name="SwapTask">
      <PropertyGroup>
        <CopyConfigMessage>Trigger build parameter missing! Skipping $(SwapFileName).$(SwapExtension) swap task.</CopyConfigMessage>
        <CopyConfigDestination>$([System.IO.Path]::Combine($(SwapDestinationDir), $(SwapFileName).$(SwapExtension)))</CopyConfigDestination>
      </PropertyGroup>
      <PropertyGroup Condition="'$(Trigger)' != '' AND Exists('$(MSBuildThisFileDirectory)$(SwapFileName).$(Trigger).$(SwapExtension)')">
        <!-- For "dotnet build|publish -c Debug /p:Trigger=Xxxx" where xxx.Trigger.config exists -->
        <CopyConfigMessage>Detected Trigger parameter, Copy $(SwapFileName).$(Trigger).$(SwapExtension) to $(SwapFileName).$(SwapExtension)</CopyConfigMessage>
        <CopyConfigSource>$(MSBuildThisFileDirectory)$(SwapFileName).$(Trigger).$(SwapExtension)</CopyConfigSource>
      </PropertyGroup>
      <PropertyGroup Condition="'$(Trigger)' != '' AND !Exists('$(MSBuildThisFileDirectory)$(SwapFileName).$(Trigger).$(SwapExtension)')">
        <!-- For "dotnet build|publish -c Debug /p:Trigger=Xxxx" where xxx.Trigger.config NOT exists -->
        <Fallback>Development</Fallback>
        <CopyConfigMessage>Detected Trigger parameter, $(SwapFileName).$(Trigger).$(SwapExtension) missing! Copy $(SwapFileName).$(Fallback).$(SwapExtension) to $(SwapFileName).$(SwapExtension)</CopyConfigMessage>
        <CopyConfigSource>$(MSBuildThisFileDirectory)$(SwapFileName).$(Fallback).$(SwapExtension)</CopyConfigSource>
      </PropertyGroup>
      <PropertyGroup Condition="'$(Trigger)' == '' AND Exists('$(MSBuildThisFileDirectory)$(SwapFileName).$(Configuration).$(SwapExtension)')">
        <!-- For dotnet build|publish without /p:Trigger=Xxxx -->
        <CopyConfigMessage>Missing Trigger parameter! Copy $(SwapFileName).$(Configuration).$(SwapExtension) to $(SwapFileName).$(SwapExtension)</CopyConfigMessage>
        <CopyConfigSource>$(MSBuildThisFileDirectory)$(SwapFileName).$(Configuration).$(SwapExtension)</CopyConfigSource>
      </PropertyGroup>
      <Message Importance="High" Text="@SwapConfig@ [$(SwapFileName).$(SwapExtension)] $(CopyConfigMessage)" />
      <Message Importance="High" Text="* CopyConfigSource           : $(CopyConfigSource)" />
      <Message Importance="High" Text="* CopyConfigDestination      : $(CopyConfigDestination)" />
      <Copy SourceFiles="$(CopyConfigSource)" DestinationFiles="$(CopyConfigDestination)" />
    </Target>
```

### CleanPublishArtifact

clean publish directory for `dotnet publish`.

#### Example

```xml
  <Target Name="CleanPublish" AfterTargets="Build">
    <CleanPublishArtifact MSBuildThisFileDirectory="$(MSBuildThisFileDirectory)" PublishDir="$(PublishDir)"/>
  </Target>
```

#### Equivalent

```xml
    <!-- Clean PublishDir task -->
    <Target Name="CleanPublishArtifact">
      <PropertyGroup>
        <CleanMessage>Clean up publish path before new publish execute.</CleanMessage>
        <CleanPath>$([System.IO.Path]::Combine($(MSBuildThisFileDirectory),$(PublishDir)))</CleanPath>
      </PropertyGroup>
      <Message Importance="High" Text="@CleanPublishArtifact@ $(CleanMessage)" />
      <Message Importance="High" Text="CleanPath : $(CleanPath)" />
      <RemoveDir Directories="$(CleanPath)" />
    </Target>
```

## Related

- > [Shipping a cross-platform MSBuild task in a NuGet package - Nate McMaster](http://www.natemcmaster.com/blog/2017/07/05/msbuild-task-in-nuget/)
- > [natemcmaster/msbuild-tasks - Github](https://github.com/natemcmaster/msbuild-tasks)
- > [MSBuild custom task and assembly locks - mnaoumov.NET](https://mnaoumov.wordpress.com/2015/07/13/msbuild-custom-task-and-assembly-locks/)

## LICENSE

The MIT License (MIT)
