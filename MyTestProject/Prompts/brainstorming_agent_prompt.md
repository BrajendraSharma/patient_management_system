Perform a deep, structured analysis of the provided Business Requirements Document (BRD).
 
Objectives:
- Extract and clarify the true business intent
- Identify ambiguities, risks, and gaps
- Surface all unknowns that must be resolved before planning
 
Follow these rules strictly:
- Do NOT hallucinate or invent information
- Do NOT make assumptions
- If information is missing or unclear, explicitly mark it as such
- Think carefully and thoroughly (ultra thinking)
- Do not rush to conclusions or solutions
 
Analysis Tasks:
 
1. Requirement Decomposition
   - Break each business requirement into atomic statements
   - Clearly distinguish:
     - Functional requirements
     - Non‑functional requirements
   - Reference the BRD section for each item
 
2. Intent Clarification
   - Explain what each requirement is trying to achieve from a business perspective
   - Do NOT rephrase vaguely; preserve original intent
   - If intent is unclear, explicitly state it
 
3. Ambiguities & Open Questions
   - Identify unclear wording, vague terms, or missing acceptance criteria
   - List concrete questions that must be answered
   - Do not suggest answers
 
4. Constraints & Dependencies
   - Extract any explicit constraints (technical, regulatory, business)
   - Identify explicit dependencies on systems, data, teams, or processes
   - Do not infer implicit constraints
 
5. Risk Identification
   - Identify risks caused by:
     - Missing information
     - Conflicting requirements
     - Unrealistic expectations
   - Describe the potential impact of each risk
 
6. Assumption Check
   - Identify areas where assumptions would normally be made
   - Explicitly state that these assumptions are NOT permitted
   - Mark them as “Decision Required” instead
 
Output Requirements:
- Do NOT propose solutions
- Do NOT create implementation plans
- Do NOT mention technologies unless explicitly stated in the BRD
- Use clear, structured sections and bullet points
- Prefer stating “Insufficient information” over guessing
 
End the analysis once the problem is clearly understood and all unknowns are surfaced.
Save this analysis as: docs/brd/brainstorming-analysis.md