
interface PandemicState {
    isGameOver: boolean;
    turnsPlayed: number;
    outbreakCount: number;
    pandemicCardCount: number;
    infectionRateMarker: number;
    researchStationStock: number;
    playerStates: ReadonlyArray<PandemicPlayerState>;
    cities: ReadonlyArray<PandemicMapNode>;
};

interface PandemicPlayerState {
    playerRole: string;
    location: string;
    playerHand: PandemicPlayerHand;
}

interface PandemicPlayerHand {
    playerCardType: string;
    value: number;
    name: string;
};

interface PandemicMapNode {
    city: string;
    connectedCities: ReadonlyArray<string>;
    hasResearchStation: boolean; 
    diseaseCubes: ReadonlyArray<PandemicDiseaseAndCount>;
    diseaseCubeCount: number;
    gridRow: number;
    gridColumn: number;
};

interface PandemicDiseaseAndCount {
    disease: string;
    count: number;
};



