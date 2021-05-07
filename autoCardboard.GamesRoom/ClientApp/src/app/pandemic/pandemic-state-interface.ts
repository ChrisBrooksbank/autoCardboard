interface PandemicState {
    isGameOver: boolean;
    turnsPlayed: number;
    outbreakCount: number;
    pandemicCardCount: number;
    infectionRateMarker: number;
    researchStationStock: number;
    playerStates: ReadonlyArray<PandemicPlayerState>;
    cities: ReadonlyArray<PandemicMapNode>;
}
interface PandemicPlayerState {
    playerRole: string;
    location: string;
    playerHand: ReadonlyArray<PandemicPlayerCard>;
}

interface PandemicPlayerCard {
    playerCardType: string;
    value: number;
    name: string;
}

interface PandemicMapNode {
    city: string;
    connectedCities: ReadonlyArray<string>;
    hasResearchStation: boolean;
    diseaseCubes: ReadonlyArray<PandemicDiseaseAndCount>;
    diseaseCubeCount: number;
    gridRow: number; // 0 is top, 0 - 6
    gridColumn: number; // 0 is left, 0 - 13
}

interface PandemicDiseaseAndCount {
    disease: string;
    count: number;
}
