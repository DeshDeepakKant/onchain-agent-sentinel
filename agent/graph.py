from langgraph.graph import StateGraph, END
from .state import AgentState
from .nodes import (
    parse_intent_node,
    payment_gate_node,
    fetch_onchain_data_node,
    evaluate_security_node,
    synthesize_report_node,
)

def route_after_payment_gate(state: AgentState):
    """Conditional Edge: Diverts flow if payment challenge is required and unpaid."""
    if state.get("payment_required") and not state.get("payment_verified"):
        return END
    if state.get("error"):
        return END
    return "fetch_onchain_data"

def route_after_fetch(state: AgentState):
    """Conditional Edge: Stops if onchain data retrieval failed."""
    if state.get("error") or not state.get("onchain_data"):
        return "synthesize_report"
    return "evaluate_security"

def build_sentinel_graph():
    """Builds and compiles the Onchain Agent Sentinel state machine."""
    workflow = StateGraph(AgentState)

    # Register Nodes
    workflow.add_node("parse_intent", parse_intent_node)
    workflow.add_node("payment_gate", payment_gate_node)
    workflow.add_node("fetch_onchain_data", fetch_onchain_data_node)
    workflow.add_node("evaluate_security", evaluate_security_node)
    workflow.add_node("synthesize_report", synthesize_report_node)

    # Define Graph Flow
    workflow.set_entry_point("parse_intent")
    workflow.add_edge("parse_intent", "payment_gate")

    workflow.add_conditional_edges(
        "payment_gate",
        route_after_payment_gate,
        {
            "fetch_onchain_data": "fetch_onchain_data",
            END: END,
        },
    )

    workflow.add_conditional_edges(
        "fetch_onchain_data",
        route_after_fetch,
        {
            "evaluate_security": "evaluate_security",
            "synthesize_report": "synthesize_report",
        },
    )

    workflow.add_edge("evaluate_security", "synthesize_report")
    workflow.add_edge("synthesize_report", END)

    return workflow.compile()
