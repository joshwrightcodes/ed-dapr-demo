{{/*
Expand the name of the chart.
*/}}
{{- define "usergroups-api.name" -}}
{{- default .Chart.Name .Values.nameOverride | trunc 63 | trimSuffix "-" }}
{{- end }}

{{/*
Create a default fully qualified app name.
We truncate at 63 chars because some Kubernetes name fields are limited to this (by the DNS naming spec).
If release name contains chart name it will be used as a full name.
*/}}
{{- define "usergroups-api.fullname" -}}
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
{{- define "usergroups-api.chart" -}}
{{- printf "%s-%s" .Chart.Name .Chart.Version | replace "+" "_" | trunc 63 | trimSuffix "-" }}
{{- end }}

{{/*
Common labels
*/}}
{{- define "usergroups-api.labels" -}}
helm.sh/chart: {{ include "usergroups-api.chart" . }}
{{ include "usergroups-api.selectorLabels" . }}
{{- if .Chart.AppVersion }}
app.kubernetes.io/version: {{ .Chart.AppVersion | quote }}
{{- end }}
app.kubernetes.io/managed-by: {{ .Release.Service }}
app.kubernetes.io/part-of: {{ default (include "usergroups-api.fullname" .) .Values.global.part_of }}
{{- if .Values.global.commonLabels}}
{{ toYaml .Values.global.commonLabels }}
{{- end }}
{{- end }}

{{/*
Selector labels
*/}}
{{- define "usergroups-api.selectorLabels" -}}
app.kubernetes.io/name: {{ include "usergroups-api.name" . }}
app.kubernetes.io/instance: {{ .Release.Name }}
{{- end }}

{{/*
Create the name of the service account to use
*/}}
{{- define "usergroups-api.serviceAccountName" -}}
{{- if .Values.serviceAccount.create }}
{{- default (include "usergroups-api.fullname" .) .Values.serviceAccount.name }}
{{- else }}
{{- default "default" .Values.serviceAccount.name }}
{{- end }}
{{- end }}

{{/*
Create Dapr annotations
*/}}
{{- define "usergroups-api.deployment.dapr.annotations" -}}
{{- if .Values.dapr -}}
dapr.io/enabled: {{ .Values.dapr.enable | quote }}
{{ if .Values.dapr.enable -}}
dapr.io/app-id: {{ include "usergroups-api.name" . | quote }}
dapr.io/config: {{ include "usergroups-api.fullname" . }}
dapr.io/log-as-json: {{ default "true" .Values.dapr.logAsJson | quote }}
dapr.io/app-port: {{ default 80 .Values.dapr.appPort | quote }}
{{- end -}}
{{- end -}}
{{- end -}}

{{/*
Create OpenTelemetry Operator annotations
*/}}
{{- define "usergroups-api.deployment.opentelemetry.annotations" -}}
{{- if .Values.openTelemetry -}}
sidecar.opentelemetry.io/inject: {{ .Values.openTelemetry.enableSideCar | quote }}
{{- end -}}
{{- end -}}
