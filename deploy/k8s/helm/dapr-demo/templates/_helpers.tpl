{{/* Expand the name of the chart. */}}
{{- define "daprdemo.name" -}}
{{- default .Chart.Name .Values.nameOverride | trunc 63 | trimSuffix "-" -}}
{{- end -}}

{{/* Create chart name and version as used by the chart label. */}}
{{- define "daprdemo.chart" -}}
{{- printf "%s-%s" .Chart.Name .Chart.Version | replace "+" "_" | trunc 63 | trimSuffix "-" -}}
{{- end -}}


{{/*
Create a default fully qualified app name.
We truncate at 63 chars because some Kubernetes name fields are limited to this (by the DNS naming spec).
*/}}
{{- define "daprdemo.fullname" -}}
{{- if .Values.fullnameOverride -}}
{{- .Values.fullnameOverride | trunc 63 | trimSuffix "-" -}}
{{- else -}}
{{- $name := default .Chart.Name .Values.nameOverride -}}
{{- if contains $name .Release.Name -}}
{{- .Release.Name | trunc 63 | trimSuffix "-" -}}
{{- else -}}
{{- printf "%s-%s" .Release.Name $name | trunc 63 | trimSuffix "-" -}}
{{- end -}}
{{- end -}}
{{- end -}}

{{/* Selector labels */}}
{{- define "daprdemo.selectorLabels" -}}
app.kubernetes.io/name: {{ include "daprdemo.name" . }}
app.kubernetes.io/instance: {{ .Release.Name }}
{{- end -}}

{{/* Common labels */}}
{{- define "daprdemo.labels" -}}
helm.sh/chart: {{ include "daprdemo.chart" . }}
{{ include "daprdemo.selectorLabels" . }}
{{- if .Chart.AppVersion }}
app.kubernetes.io/version: {{ .Chart.AppVersion | quote }}
{{- end }}
app.kubernetes.io/part-of: {{ template "daprdemo.name" . }}
app.kubernetes.io/managed-by: {{ .Release.Service }}
{{- if .Values.commonLabels}}
{{ toYaml .Values.commonLabels }}
{{- end }}
{{- end -}}

{{- define "daprdemo.usergroups.api.name" -}}
{{- default "usergroups-api" .Values.userGroups.api.name | trunc 63 | trimSuffix "-" -}}
{{- end -}}

{{- define "daprdemo.mailhog.name" -}}
{{- default "mailhog" .Values.mailhog.nameOverride | trunc 63 | trimSuffix "-" -}}
{{- end -}}

{{/*
Create a default fully qualified app name.
We truncate at 63 chars because some Kubernetes name fields are limited to this (by the DNS naming spec).
If release name contains chart name it will be used as a full name.
*/}}
{{- define "daprdemo.mailhog.fullname" -}}
{{- if .Values.mailhog.fullnameOverride -}}
{{- .Values.mailhog.fullnameOverride | trunc 63 | trimSuffix "-" -}}
{{- else -}}
{{- $daprmailname := default "mailhog" .Values.mailhog.nameOverride -}}
{{- printf "%s-%s" .Release.Name $daprmailname | trunc 63 | trimSuffix "-" -}}
{{- end -}}
{{- end -}}


{{/*
Create a default fully qualified app name.
We truncate at 63 chars because some Kubernetes name fields are limited to this (by the DNS naming spec).
If release name contains chart name it will be used as a full name.
*/}}
{{- define "daprdemo.opentelemetry_collector.fullname" -}}
{{- if index .Values "opentelemetry-collector" "fullnameOverride" }}
{{- index .Values "opentelemetry-collector" "fullnameOverride" | trunc 63 | trimSuffix "-" }}
{{- else }}
{{ $nameOverride := index .Values "opentelemetry-collector" "nameOverride" }}
{{- $name := default "opentelemetry-collector" $nameOverride }}
{{- if contains $name .Release.Name }}
{{- .Release.Name | trunc 63 | trimSuffix "-" }}
{{- else }}
{{- printf "%s-%s" .Release.Name $name | trunc 63 | trimSuffix "-" }}
{{- end }}
{{- end }}
{{- end }}