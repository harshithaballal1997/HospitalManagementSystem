# Epics & User Stories: HMS AI Enhancements

## Epic 1: Clinical EMR Foundation
Foundation for structured clinical data entry.

### Story 1.1: Lab & Vitals Management
As a doctor, I want to record patient vitals and lab results in a structured format so that data is available for AI analysis.
**Acceptance Criteria:**
- CRUD for Lab Reports.
- Entry for BP, Pulse, Weight, Height.
- Real-time BMI calculation.

## Epic 2: Medical Assistant AI
AI-driven insights to solve the "Ritual of Re-Telling".

### Story 2.1: Patient History Summarization
As a doctor, I want an AI-generated summary of patient history so that I can quickly understand their clinical state.
**Acceptance Criteria:**
- "AI Summary" button on Labs dashboard.
- Modal displaying structured summary.

## Epic 3: Patient Safety AI
Real-time safety guardrails for prescribing.

### Story 3.1: Prescription Safety Checker
As a doctor, I want the system to check for drug interactions and allergies during clinical notes entry so that I can prevent medical errors.
**Acceptance Criteria:**
- "Check AI Safety" button in Lab creation.
- Real-time detection of drug-drug and drug-allergy conflicts.

## Epic 4: IoT Hospital Operations
Automation of physical hospital status.

### Story 4.1: Automated Bed Status
As a nurse, I want bed status to update automatically based on patient presence so that ADT information is always accurate.
**Acceptance Criteria:**
- BedStatusHandler for sensor input.
- Automatic update of Room status.

## Epic 5: Advanced Vision AI (Future)
Automated diagnostic analysis.

### Story 5.1: MRI Shadow-Talker
As a radiologist, I want AI to identify anomalies in MRI scans so that I can prioritize critical cases.
**Acceptance Criteria:**
- Vision-LLM integration for MRI analysis.
