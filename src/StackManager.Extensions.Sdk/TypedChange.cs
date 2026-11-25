using System.Text.Json.Serialization;

namespace StackManager.Extensions;

/// <summary>
/// One change with a fixed form. The host builds the new text of the file itself.
/// The API accepts no free text commands.
/// </summary>
[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(SetPhpIni), "set_php_ini")]
[JsonDerivedType(typeof(EnablePhpExtension), "enable_php_extension")]
[JsonDerivedType(typeof(SetApacheDirective), "set_apache_directive")]
[JsonDerivedType(typeof(EnableApacheModule), "enable_apache_module")]
[JsonDerivedType(typeof(AddVirtualHost), "add_virtual_host")]
[JsonDerivedType(typeof(SetMariaDbOption), "set_mariadb_option")]
public abstract record TypedChange;

/// <summary>Sets a value in php.ini, for example upload_max_filesize = 64M.</summary>
public sealed record SetPhpIni(string Key, string Value) : TypedChange;

/// <summary>Enables a PHP extension in php.ini, for example intl.</summary>
public sealed record EnablePhpExtension(string Name) : TypedChange;

/// <summary>Sets a single-value Apache directive in httpd.conf, for example ServerName.</summary>
public sealed record SetApacheDirective(string Name, string Value) : TypedChange;

/// <summary>Enables an Apache module, for example rewrite.</summary>
public sealed record EnableApacheModule(string Name) : TypedChange;

/// <summary>Adds a virtual host for a development site.</summary>
public sealed record AddVirtualHost(string ServerName, string DocumentRoot, int Port = 80) : TypedChange;

/// <summary>Sets an option in the [mysqld] section of my.ini, for example max_allowed_packet.</summary>
public sealed record SetMariaDbOption(string Key, string Value) : TypedChange;
