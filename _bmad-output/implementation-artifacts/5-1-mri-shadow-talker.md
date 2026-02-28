# Story 5.1: MRI Shadow-Talker

Status: ready-for-dev

## Story

As a Radiologist,
I want AI to identify anomalies in MRI scans,
so that I can prioritize critical cases.

## Acceptance Criteria

1. **Vision-LLM Integration:** Successfully integrate a Multimodal Large Language Model (e.g., GPT-4o with Vision or LLaVA-Med) to process MRI images.
2. **Anomaly Identification:** The AI must detect potential anomalies (tumors, lesions, etc.) and provide a preliminary natural language description.
3. **Prioritization Logic:** The system must flag critical findings for immediate review in the doctor's dashboard.
4. **Clinical EMR Integration:** Findings must be linked to the existing `PatientReport` and `Lab` entities.
5. **Secure Storage Compliance:** Scans must be retrieved from and processed according to the patient-specific paths in `Uploads/Diagnostics/`.

## Tasks / Subtasks

- [ ] **Technical Setup (AC: 1)**
  - [ ] Implement `IVisionAssistantService` interface.
  - [ ] Create `VisionAssistantService` with LLM API integration.
- [ ] **AI Analysis Pipeline (AC: 2, 5)**
  - [ ] Implement image pre-processing for MRI formats.
  - [ ] Integrate prompts tailored for medical anomaly detection (based on research-backed medical VLM adapting).
- [ ] **UI Integration (AC: 3)**
  - [ ] Update Clinical Dashboard to show "AI Anomaly Flags".
  - [ ] Create detailed "Vision Insights" view for scanned images.
- [ ] **Data Persistence (AC: 4)**
  - [ ] Update `PatientReport` to include `AiVisionSummary` and `AnomalySensitivityScore`.

## Dev Notes

### Technical Stack & Constraints
- **Model:** Recommend GPT-4o-vision-preview or LLaVA-Med for clinical reasoning.
- **Privacy:** Implement LoRA-based fine-tuning if using local models to maintain data sovereignty.
- **Explainability:** Ensure the AI output includes the specific anatomical regions where anomalies were detected.
- **Performance:** Asynchronous processing for image analysis to prevent UI blocking.

### Source Tree Components
- **Services:** [IVisionAssistantService.cs](file:///c:/Repo/Hospital/Hospital.Services/IVisionAssistantService.cs), [VisionAssistantService.cs](file:///c:/Repo/Hospital/Hospital.Services/VisionAssistantService.cs)
- **Controllers:** [LabsController.cs](file:///c:/Repo/Hospital/Hospital.Web/Areas/Doctor/Controllers/LabsController.cs)
- **Models:** [PatientReport.cs](file:///c:/Repo/Hospital/Hospital.Models/PatientReport.cs)
- **Utilities:** [ImageOperations.cs](file:///c:/Repo/Hospital/Hospital.Utilities/ImageOperations.cs)

### References
- [PRD Section 2](file:///c:/Repo/Hospital/_bmad-output/planning-artifacts/prd.md#Feature-Analysis)
- [Epic 5.1](file:///c:/Repo/Hospital/_bmad-output/planning-artifacts/epics.md#Epic-5)
- [Diagnostic Storage Logic](file:///c:/Repo/Hospital/Hospital.Utilities/ImageOperations.cs)

## Dev Agent Record

### Agent Model Used
Claude 3.5 Sonnet / Gemini 1.5 Pro

### Debug Log References
- Story initialized: 2026-02-22
- Context analysis: Research-backed Vision-LLM medical domain adaptation applied.
