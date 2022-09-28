{{/*
Expand the name of the chart.
*/}}
{{- define "courses-api.name" -}}
{{- default .Chart.Name .Values.nameOverride | trunc 63 | trimSuffix "-" }}
{{- end }}

{{/*
Create a default fully qualified app name.
We truncate at 63 chars because some Kubernetes name fields are limited to this (by the DNS naming spec).
If release name contains chart name it will be used as a full name.
*/}}
{{- define "courses-api.fullname" -}}
{{- if .Values.fullnameOverride }}
{{- .Values.fullnameOverride | trunc 63 | trimSuffix "-" }}
{{- else }}
{{- $name := default .Chart.Name .Values.nameOverride }}
{{- if contains $name .Release.Name }}
{{- .Release.Name | trunc 63 | trimSuffix "-" }}
{{- else }}
{{- printf "%s-%s" .Release.Name $name | trunc 63 | trimSuffix "-" }}
{{- end }}
{{- end }}
{{- end }}

{{/*
Create chart name and version as used by the chart label.
*/}}
{{- define "courses-api.chart" -}}
{{- printf "%s-%s" .Chart.Name .Chart.Version | replace "+" "_" | trunc 63 | trimSuffix "-" }}
{{- end }}

{{/*
Common labels
*/}}
{{- define "courses-api.labels" -}}
helm.sh/chart: {{ include "courses-api.chart" . }}
{{ include "courses-api.selectorLabels" . }}
{{- if .Chart.AppVersion }}
app.kubernetes.io/version: {{ .Chart.AppVersion | quote }}
{{- end }}
app.kubernetes.io/managed-by: {{ .Release.Service }}
app.kubernetes.io/part-of: {{ default (include "courses-api.fullname" .) .Values.global.part_of }}
{{- if .Values.global.commonLabels}}
{{ toYaml .Values.global.commonLabels }}
{{- end }}
{{- end }}

{{/*
Selector labels
*/}}
{{- define "courses-api.selectorLabels" -}}
app.kubernetes.io/name: {{ include "courses-api.name" . }}
app.kubernetes.io/instance: {{ .Release.Name }}
{{- end }}

{{/*
Create the name of the service account to use
*/}}
{{- define "courses-api.serviceAccountName" -}}
{{- if .Values.serviceAccount.create }}
{{- default (include "courses-api.fullname" .) .Values.serviceAccount.name }}
{{- else }}
{{- default "default" .Values.serviceAccount.name }}
{{- end }}
{{- end }}

{{/*
Create Dapr annotations
*/}}
{{- define "courses-api.deployment.dapr.annotations" -}}
{{- if .Values.dapr -}}
dapr.io/enabled: {{ .Values.dapr.enable | quote }}
{{ if .Values.dapr.enable -}}
dapr.io/app-id: {{ include "courses-api.name" . | quote }}
dapr.io/config: {{ include "courses-api.fullname" . }}
dapr.io/log-as-json: {{ default "true" .Values.dapr.logAsJson | quote }}
dapr.io/app-port: {{ default 80 .Values.dapr.appPort | quote }}
{{- end -}}
{{- end -}}
{{- end -}}

{{/*
Create OpenTelemetry Operator annotations
*/}}
{{- define "courses-api.deployment.opentelemetry.annotations" -}}
{{- if .Values.openTelemetry -}}
sidecar.opentelemetry.io/inject: {{ .Values.openTelemetry.enableSideCar | quote }}
{{- end -}}
{{- end -}}
