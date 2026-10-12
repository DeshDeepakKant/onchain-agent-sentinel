import re
from typing import Optional, Tuple
from blockchain.rpc import BlockchainClient

ETH_ADDRESS_PATTERN = re.compile(r"0x[a-fA-F0-9]{40}")

def extract_ethereum_address(text: str) -> Optional[str]:
    """Finds first matching Ethereum address from text."""
    match = ETH_ADDRESS_PATTERN.search(text)
    return match.group(0) if match else None

def inspect_address_onchain(address: str, network: str = "mainnet") -> dict:
    """Invokes the RPC client to query address details."""
    client = BlockchainClient(network=network)
    return client.get_address_overview(address)
