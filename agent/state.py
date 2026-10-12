from typing import TypedDict, Optional, List, Dict, Any

class AgentState(TypedDict, total=False):
    # Input queries & identity
    query: str
    caller_id: str
    payment_proof: Optional[str]

    # Parsed parameters
    target_address: Optional[str]
    network: str
    task_type: str  # 'inspect', 'risk_audit', 'transfer_check'

    # Gate & Execution status
    payment_required: bool
    payment_verified: bool
    payment_challenge: Optional[Dict[str, Any]]

    # Onchain state
    onchain_data: Optional[Dict[str, Any]]
    security_verdict: Optional[Dict[str, Any]]

    # Execution traceability
    logs: List[str]
    final_output: Optional[Dict[str, Any]]
    error: Optional[str]
