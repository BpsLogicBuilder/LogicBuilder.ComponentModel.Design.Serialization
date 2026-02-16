# LogicBuilder.ComponentModel.Design.Serialization

[![Build Status](https://github.com/BpsLogicBuilder/LogicBuilder.ComponentModel.Design.Serialization/actions/workflows/ci.yml/badge.svg)](https://github.com/BpsLogicBuilder/LogicBuilder.ComponentModel.Design.Serialization/actions/workflows/ci.yml)
[![CodeQL](https://github.com/BpsLogicBuilder/LogicBuilder.ComponentModel.Design.Serialization/actions/workflows/github-code-scanning/codeql/badge.svg)](https://github.com/BpsLogicBuilder/LogicBuilder.ComponentModel.Design.Serialization/actions/workflows/github-code-scanning/codeql)
[![codecov](https://codecov.io/gh/BpsLogicBuilder/LogicBuilder.ComponentModel.Design.Serialization/graph/badge.svg?token=PFDB2PEVZT)](https://codecov.io/gh/BpsLogicBuilder/LogicBuilder.ComponentModel.Design.Serialization)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=BpsLogicBuilder_LogicBuilder.ComponentModel.Design.Serialization&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=BpsLogicBuilder_LogicBuilder.ComponentModel.Design.Serialization)
[![NuGet](https://img.shields.io/nuget/v/LogicBuilder.ComponentModel.Design.Serialization.svg)](https://www.nuget.org/packages/LogicBuilder.ComponentModel.Design.Serialization)

## Serialization Management

The library implements the core methods for serialization management like:

- `CreateInstance() `: Object creation
- `GetInstance()`:  Object retrieval
- `GetName() / SetName()`: Object naming
- `GetService()`: Service provider functionality
- `AddSerializationProvider() / RemoveSerializationProvider()` Provider management

The WorkflowMarkupSerializationManager in LogicBuilder.Workflow.ComponentModel.Serialization itself implements IDesignerSerializationManager and leverages the core implementations from DesignerSerializationManager using the Decorator pattern.

## Contributing

Contributions are welcome.