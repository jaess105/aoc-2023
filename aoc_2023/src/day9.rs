pub fn solve_a(test_input: String) -> i32 {
    let parsed_input: Vec<Vec<i32>> = parse_input(test_input);
    parsed_input.iter().map(extrapolate_next_val).sum()
}

pub fn solve_b(test_input: String) -> i32 {
    let parsed_input: Vec<Vec<i32>> = parse_input(test_input);
    parsed_input.iter().map(extrapolate_next_val_b).sum()
}

fn parse_input(test_input: String) -> Vec<Vec<i32>> {
    test_input
        .split("\n")
        .map(|s| {
            s.split(" ")
                .map(|num| num.parse::<i32>().unwrap())
                .collect()
        })
        .collect()
}

fn extrapolate_next_val(sequence: &Vec<i32>) -> i32 {
    let mut current_seq = sequence;
    let mut all_seq: Vec<Vec<i32>> = vec![sequence.clone()];
    while current_seq.iter().any(|i| *i != 0) {
        let next_seq = current_seq
            .windows(2)
            .map(|tuple| tuple[1] - tuple[0])
            .collect();
        all_seq.push(next_seq);
        current_seq = all_seq.last().unwrap();
    }

    all_seq
        .iter()
        .rev()
        .fold(0, |prev, next| prev + next.last().unwrap())
}

fn extrapolate_next_val_b(sequence: &Vec<i32>) -> i32 {
    let mut current_seq = sequence;
    let mut all_seq: Vec<Vec<i32>> = vec![sequence.clone()];
    while current_seq.iter().any(|i| *i != 0) {
        let next_seq = current_seq
            .windows(2)
            .map(|tuple| tuple[1] - tuple[0])
            .collect();
        all_seq.push(next_seq);
        current_seq = all_seq.last().unwrap();
    }


    all_seq.iter_mut().for_each(|x| x.reverse());

    all_seq
        .iter()
        .rev()
        .fold(0, |prev, next| next.last().unwrap() - prev)
}

#[cfg(test)]
mod tests {
    use super::{solve_a, solve_b};

    const TEST_INPUT: &str = "0 3 6 9 12 15
1 3 6 10 15 21
10 13 16 21 30 45";

    #[test]
    fn test_a() {
        let result = solve_a(TEST_INPUT.into());
        assert_eq!(result, 114)
    }

    #[test]
    fn test_b() {
        let result = solve_b(TEST_INPUT.into());
        assert_eq!(result, 2)
    }
}
