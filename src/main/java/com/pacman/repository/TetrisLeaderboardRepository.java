package com.pacman.repository;

import com.pacman.entity.TetrisLeaderboard;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.Query;
import org.springframework.stereotype.Repository;

import java.util.Optional;

@Repository
public interface TetrisLeaderboardRepository extends JpaRepository<TetrisLeaderboard, Long> {
    // 按分数排序获取排行榜
    java.util.List<TetrisLeaderboard> findAllByOrderByScoreDesc();
    
    // 获取用户的最佳成绩
    Optional<TetrisLeaderboard> findFirstByUserIdOrderByScoreDesc(Long userId);
    
    // 获取排名
    @Query(value = "SELECT COUNT(*) + 1 FROM tetris_leaderboard WHERE score > :score", nativeQuery = true)
    Integer getRank(Integer score);
}