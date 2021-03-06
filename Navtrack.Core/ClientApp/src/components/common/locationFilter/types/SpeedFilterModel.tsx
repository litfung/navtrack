import { NumberFilterType } from "./NumberFilterType";
import { ComparisonType } from "./ComparisonType";

export type SpeedFilterModel = {
  single: number;
  min: number;
  max: number;
  numberFilterType: NumberFilterType;
  comparisonType: ComparisonType;
  enabled: boolean;
};

export const DefaultSpeedFilterModel: SpeedFilterModel = {
  single: 50,
  min: 50,
  max: 130,
  numberFilterType: NumberFilterType.Single,
  comparisonType: ComparisonType.GreaterThan,
  enabled: false
};

export const speedFilterToString = (filter: SpeedFilterModel) => {
  const comparisonText = {
    [ComparisonType.GreaterThan]: ">",
    [ComparisonType.Equals]: "=",
    [ComparisonType.LessThan]: "<"
  };

  if (filter.numberFilterType === NumberFilterType.Single) {
    return `${comparisonText[filter.comparisonType]} ${filter.single} km/h`;
  } else {
    return `${filter.min} km/h - ${filter.max} km/h`;
  }
};
