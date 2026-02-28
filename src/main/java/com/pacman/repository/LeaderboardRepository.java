package com.pacman.repository;

import com.pacman.entity.Leaderboard;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.Query;
import org.springframework.stereotype.Repository;

import java.util.List;
import java.util.Optional;

@Repository
public interface LeaderboardRepository extends JpaRepository<Leaderboard, Long> {
    // 按分数和用时排序获取排行榜
    List<Leaderboard> findAllByOrderByScoreDescTimeUsedAsc();
    
    // 获取用户的最佳成绩
    Optional<Leaderboard> findFirstByUserIdOrderByScoreDescTimeUsedAsc(Long userId);
    
    // 获取排名
    @Query(value = "SELECT COUNT(*) + 1 FROM leaderboard WHERE (score > :score) OR (score = :score AND time_used < :timeUsed)", nativeQuery = true)
    Integer getRank(Integer timeUsed, Integer score);
}
