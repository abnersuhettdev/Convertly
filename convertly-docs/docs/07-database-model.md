# 07 — Modelo de Banco de Dados

Banco oficial:

```txt
Supabase PostgreSQL
```

ORM:

```txt
Entity Framework Core 8
```

## users

```txt
id uuid primary key
name varchar(120) not null
email varchar(180) not null unique
password_hash text not null
created_at timestamp not null
updated_at timestamp null
is_active boolean not null default true
```

## refresh_tokens

```txt
id uuid primary key
user_id uuid not null
token_hash text not null
expires_at timestamp not null
revoked_at timestamp null
created_at timestamp not null
```

## plans

```txt
id uuid primary key
name varchar(50) not null
slug varchar(50) not null unique
monthly_conversion_limit int not null
max_file_size_mb int not null
retention_hours int not null
price_cents int not null
is_active boolean not null
created_at timestamp not null
```

Seeds:

```txt
Free      | free     | 5   | 10  | 24  | 0
Pro       | pro      | 100 | 50  | 168 | 1990
Business  | business | 500 | 200 | 720 | 4990
```

## user_subscriptions

```txt
id uuid primary key
user_id uuid not null
plan_id uuid not null
status int not null
started_at timestamp not null
ends_at timestamp null
created_at timestamp not null
updated_at timestamp null
```

## monthly_usages

```txt
id uuid primary key
user_id uuid not null
year int not null
month int not null
conversions_used int not null default 0
created_at timestamp not null
updated_at timestamp null
```

Restrição:

```txt
unique(user_id, year, month)
```

## file_assets

```txt
id uuid primary key
user_id uuid not null
original_file_name varchar(255) not null
stored_file_name varchar(255) not null
storage_path text not null
bucket_name varchar(120) not null
extension varchar(20) not null
mime_type varchar(120) not null
size_bytes bigint not null
kind int not null
created_at timestamp not null
expires_at timestamp null
```

## conversion_jobs

```txt
id uuid primary key
user_id uuid not null
source_file_id uuid not null
output_file_id uuid null
source_format varchar(20) not null
target_format varchar(20) not null
status int not null
error_message text null
usage_reserved boolean not null default true
created_at timestamp not null
started_at timestamp null
completed_at timestamp null
expires_at timestamp null
```

## Transações importantes

- criação de usuário + assinatura Free;
- criação de conversão + reserva de uso + file_asset + job;
- conclusão de job + file_asset converted + status Completed;
- falha de job + devolução de uso.
